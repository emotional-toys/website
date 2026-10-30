using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using AeonVoice;

var repoRoot = FindRepoRoot(Directory.GetCurrentDirectory());
var defaultInput = Path.Combine(repoRoot, "docs", "myth-henry-the-supertoy");
var defaultOutput = Path.Combine(repoRoot, "stories", "henry", "henry-aeonvoice.wav");

string inputPath = defaultInput;
string outputPath = defaultOutput;
string voiceProfile = "Leena";
string cadenceMode = "adaptive";

for (int i = 0; i < args.Length; i++)
{
    switch (args[i])
    {
        case "--input" when i + 1 < args.Length:
            inputPath = args[++i];
            break;
        case "--output" when i + 1 < args.Length:
            outputPath = args[++i];
            break;
        case "--voice" when i + 1 < args.Length:
            voiceProfile = args[++i];
            break;
        case "--cadence" when i + 1 < args.Length:
            cadenceMode = args[++i];
            break;
        case "--help":
        case "-h":
            PrintUsage(defaultInput, defaultOutput);
            return 0;
    }
}

if (!File.Exists(inputPath))
{
    Console.Error.WriteLine($"Input file not found: {inputPath}");
    return 1;
}

var text = File.ReadAllText(inputPath).Trim();
if (string.IsNullOrWhiteSpace(text))
{
    Console.Error.WriteLine($"Input file is empty: {inputPath}");
    return 1;
}

Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(outputPath))!);

try
{
    PreloadAeonVoiceNativeLibraries();
    using var engine = new AeonVoiceEngine();
    SynthesisResult result = cadenceMode.Equals("raw", StringComparison.OrdinalIgnoreCase)
        ? engine.SynthesizeToPcm16(PrepareForNarration(text), voiceProfile)
        : SynthesizeWithCadence(engine, text, voiceProfile);
    result.WriteWave(outputPath);

    Console.WriteLine($"Synthesized narration: {outputPath}");
    Console.WriteLine($"SampleRate: {result.SampleRate} Hz, Samples: {result.Samples.Length}");
    return 0;
}
catch (Exception ex)
{
    Console.Error.WriteLine("AeonVoice synthesis failed.");
    Console.Error.WriteLine(ex.Message);
    return 1;
}

static void PreloadAeonVoiceNativeLibraries()
{
    var baseDir = AppContext.BaseDirectory;
    var libs = new[]
    {
        "libAeonVoice_core.so.10",
        "libAeonVoice_audio.so.2",
        "libAeonVoice.so"
    };

    foreach (var lib in libs)
    {
        var fullPath = Path.Combine(baseDir, lib);
        if (!File.Exists(fullPath))
        {
            continue;
        }

        NativeLibrary.Load(fullPath);
    }
}

static void PrintUsage(string defaultInput, string defaultOutput)
{
    Console.WriteLine("Cadencer usage:");
    Console.WriteLine("  dotnet run --project synthesizer/cadencer -- [--input <path>] [--output <path>] [--voice <profile>] [--cadence adaptive|raw]");
    Console.WriteLine($"  Default input:  {defaultInput}");
    Console.WriteLine($"  Default output: {defaultOutput}");
    Console.WriteLine("  Default voice:  Leena");
    Console.WriteLine("  Default cadence: adaptive");
}

static string FindRepoRoot(string start)
{
    var dir = new DirectoryInfo(start);
    while (dir is not null)
    {
        var hasIndex = File.Exists(Path.Combine(dir.FullName, "index.html"));
        var hasAssets = Directory.Exists(Path.Combine(dir.FullName, "assets"));
        var hasDocs = Directory.Exists(Path.Combine(dir.FullName, "docs"));
        if (hasIndex && hasAssets && hasDocs)
        {
            return dir.FullName;
        }

        dir = dir.Parent;
    }

    return start;
}

static SynthesisResult SynthesizeWithCadence(AeonVoiceEngine engine, string text, string voiceProfile)
{
    var segments = BuildCadenceSegments(text);
    var mergedSamples = new List<short>(64_000);
    int sampleRate = 0;

    foreach (var segment in segments)
    {
        if (!string.IsNullOrWhiteSpace(segment.Text))
        {
            var partial = engine.SynthesizeToPcm16(PrepareForNarration(segment.Text), voiceProfile);
            if (sampleRate == 0)
            {
                sampleRate = partial.SampleRate;
            }
            else if (sampleRate != partial.SampleRate)
            {
                throw new InvalidOperationException("AeonVoice returned mixed sample rates across segments.");
            }

            mergedSamples.AddRange(partial.Samples);
        }

        if (segment.PauseMs > 0 && sampleRate > 0)
        {
            var silenceSamples = (int)Math.Round(sampleRate * segment.PauseMs / 1000.0);
            if (silenceSamples > 0)
            {
                mergedSamples.AddRange(new short[silenceSamples]);
            }
        }
    }

    if (sampleRate == 0 || mergedSamples.Count == 0)
    {
        throw new InvalidOperationException("No audio produced from input text.");
    }

    return new SynthesisResult(sampleRate, mergedSamples.ToArray());
}

static List<CadenceSegment> BuildCadenceSegments(string text)
{
    var segments = new List<CadenceSegment>();
    var paragraphs = Regex.Split(text, @"\r?\n\s*\r?\n")
        .Select(p => Regex.Replace(p.Trim(), @"\s+", " "))
        .Where(p => !string.IsNullOrWhiteSpace(p))
        .ToList();

    for (int i = 0; i < paragraphs.Count; i++)
    {
        var sentences = Regex.Split(paragraphs[i], @"(?<=[.!?])\s+")
            .Select(s => s.Trim())
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .ToList();

        for (int j = 0; j < sentences.Count; j++)
        {
            var sentence = sentences[j];
            var pauseMs = CalculatePauseMs(sentence);
            segments.Add(new CadenceSegment(sentence, pauseMs));
        }

        if (i < paragraphs.Count - 1)
        {
            segments.Add(new CadenceSegment("", 350));
        }
    }

    return segments;
}

static int CalculatePauseMs(string sentence)
{
    var ending = sentence.TrimEnd().LastOrDefault();
    var wordCount = Regex.Matches(sentence, @"[\p{L}\p{N}']+").Count;
    var lengthBoost = Math.Clamp((wordCount - 10) * 7, 0, 140);

    var basePause = ending switch
    {
        '?' => 520,
        '!' => 500,
        ';' => 360,
        ':' => 360,
        '.' => 430,
        ',' => 280,
        _ => 320
    };

    return basePause + lengthBoost;
}

static string PrepareForNarration(string text)
{
    // Keep the Supertoys brand intact in authored text while giving the voice
    // engine an unambiguous spoken form. Some voices can otherwise soften the
    // /t/ in "Supertoy" toward "Superboy".
    return Regex.Replace(text, @"\bSupertoy\b", "Super toy", RegexOptions.IgnoreCase);
}

internal sealed record CadenceSegment(string Text, int PauseMs);
