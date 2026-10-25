# Supertoys

**Stories and research companions for emotional exploration.**

[Supertoys](https://emotional.toys/) is a listening room and an ongoing study of the small signals that make a companion feel present: a pause, a voice, a gesture, a familiar shape. We make self-produced audio stories alongside animal-inspired research prototypes—not as replacements for human care, but as invitations to reflect on ourselves, one another, and the emotional lives we build around technology.

## Listen

The site currently features two stories in its listening room:

- **[The Star Mouse](https://emotional.toys/#stories)** — Fredric Brown’s cosmic tale, presented as a 48-minute audio story.
- **[Toptygin](https://emotional.toys/#stories)** — a short Supertoys narration produced with [AeonVoice](https://github.com/cartheur-voice/aeon-voice).

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

The site includes a local AeonVoice-based synthesizer for story narration audio.

- Story input (default): `docs/myth-henry-the-supertoy`
- Narration output (default): `assets/myth-henry-aeonvoice.wav`
- Website playback integration: `index.html` (the Stories section; native controls, no automatic playback)
- Synthesizer project: `synthesizer/story-audio-synth`
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
- `stories/toptygin/toptygin.jpg` — story cover artwork
- `assets/myth-henry-aeonvoice.wav` — *Toptygin*

### Maintenance notes

- The synthesizer depends on `AeonVoice` and `AeonVoice.Native` NuGet packages.
- Native libraries are copied by `synthesizer/story-audio-synth/AeonVoiceSynth.csproj`.
- If narration fails to load, first verify that the referenced audio file exists, then reload the page.
