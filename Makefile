STORY ?= docs/myth-henry-the-supertoy
OUTPUT ?= stories/henry/Henry - AeonVoice.wav
CADENCE ?= adaptive
VOICE_PROFILE ?= Leena

.PHONY: story-audio story-audio-default

story-audio:
	VOICE_PROFILE="$(VOICE_PROFILE)" CADENCE_MODE="$(CADENCE)" ./scripts/synthesize-story-audio.sh "$(STORY)" "$(OUTPUT)"

story-audio-default:
	VOICE_PROFILE="$(VOICE_PROFILE)" CADENCE_MODE="$(CADENCE)" ./scripts/synthesize-story-audio.sh
