# Supertoys

**Stories and research companions for emotional exploration.**

[Supertoys](https://emotional.toys/) is a listening room and an ongoing study of the small signals that make a companion feel present: a pause, a voice, a gesture, a familiar shape. We make self-produced audio stories alongside animal-inspired research prototypes—not as replacements for human care, but as invitations to reflect on ourselves, one another, and the emotional lives we build around technology.

## Listen

The site currently features two stories in its listening room:

- **[The Star Mouse](https://emotional.toys/#stories)** — Fredric Brown’s cosmic tale, presented as a 48-minute audio story.
- **[The Myth of Henry the Supertoy](https://emotional.toys/#stories)** — a short Supertoys narration about Henry, a Supertoy bear, produced with [AeonVoice](https://github.com/cartheur-voice/aeon-voice).

Each story has its own artwork and browser-native playback controls, so listeners can settle in without leaving the page.

## Meet the companions

Supertoys is also home to three exploratory companion studies:

- **dot** — attention cues and gentle reflection
- **pavie** — comfort, trust, and huggable presence
- **letty** — routines, prompts, and long-life resilience

They are research artifacts, not clinical products. The work is guided by privacy, repairability, emotional clarity, and humane boundaries. Explore the [site](https://emotional.toys/) to meet them and read about the SMTWAA and KSEM research context.

## For collaborators

This repository contains the static site for [emotional.toys](https://emotional.toys/). The technical narration handoff below is retained for maintainers and internal agents.

---

## Internal narration handoff

The site includes a cadence-focused story synthesizer that builds against the local [AeonVoice source](https://github.com/cartheur-voice/aeon-voice), rather than a published NuGet package. In the standard workspace layout, that checkout is `../../voice/aeon-voice`. AeonVoice owns engine synthesis and WAV encoding; this project owns story segmentation and deliberate pauses.

- Story input (default): `docs/myth-henry-the-supertoy`
- Narration output (default): `stories/henry/Henry - AeonVoice.wav`
- Website playback integration: `index.html` (the Stories section; native controls, no automatic playback)
- Cadence project: `synthesizer/cadencer`
- Regeneration script: `scripts/synthesize-story-audio.sh`

### Regenerate narration

Default regeneration:

```bash
make story-audio-default
```

Custom story/output:

```bash
make story-audio STORY=docs/<story-file> OUTPUT=assets/<story-audio>.wav
```

Cadence modes:

- `CADENCE=adaptive` (default): sentence-by-sentence synthesis plus punctuation and length pauses.
- `CADENCE=raw`: single-pass synthesis without cadence shaping.

Example:

```bash
make story-audio STORY=docs/my-new-story OUTPUT=assets/my-new-story.wav CADENCE=adaptive
```

Voice profile override:

```bash
VOICE_PROFILE=Leena make story-audio-default
```

### Published story assets

- `stories/star-mouse/The Star Mouse - AeonVoice.mp3` — *The Star Mouse*
- `stories/star-mouse/star-mouse.jpg` — story cover artwork
- `stories/henry/henry-guardian-night.png` — story cover artwork
- `stories/henry/Henry - AeonVoice.wav` — mastered web playback for *The Myth of Henry the Supertoy*

### Maintenance notes

- Build the local AeonVoice Linux runtime before synthesizing; the story tool copies its current native libraries and the Leena/English resources into its output at build time.
- If AeonVoice lives elsewhere, pass its checkout path with `dotnet run --project synthesizer/cadencer -p:AeonVoiceSourceRoot=/absolute/path/to/aeon-voice -- ...`.
- Cadence is implemented in `synthesizer/cadencer/Program.cs`; evolve its pause and segmentation rules there.
- `PrepareForNarration` keeps the branded spelling `Supertoy` in source text while passing the unambiguous spoken form “Super toy” to the voice engine.
- If narration fails to load, first verify that the referenced audio file exists, then reload the page.
