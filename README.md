Website for `emotional.toys`.

## Story Narration Workflow (For Next Agent)

The site includes a local AeonVoice-based synthesizer for story narration audio.

- Story input (default): `docs/myth-henry-the-supertoy`
- Narration output (default): `assets/myth-henry-aeonvoice.wav`
- Website playback integration: `index.html` (`#mythNarration`, loop enabled)
- Synthesizer project: `synthesizer/story-audio-synth`
- Regeneration script: `scripts/synthesize-story-audio.sh`

### Regenerate Narration

Default regeneration:

```bash
make story-audio-default
```

Custom story/output:

```bash
make story-audio STORY=docs/<story-file> OUTPUT=assets/<story-audio>.wav
```

Cadence modes:

- `CADENCE=adaptive` (default): sentence-by-sentence synthesis + punctuation/length pauses.
- `CADENCE=raw`: single-pass synthesis without cadence shaping.

Example:

```bash
make story-audio STORY=docs/my-new-story OUTPUT=assets/my-new-story.wav CADENCE=adaptive
```

Voice profile override:

```bash
VOICE_PROFILE=Leena make story-audio-default
```

### Notes

- The synthesizer depends on `AeonVoice` and `AeonVoice.Native` NuGet packages.
- Native libraries are copied by `synthesizer/story-audio-synth/AeonVoiceSynth.csproj`.
- If narration fails to load on site, first verify the output WAV exists at the expected path and then reload the page.
