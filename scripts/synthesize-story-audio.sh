#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"

STORY_PATH="${1:-$ROOT_DIR/docs/myth-henry-the-supertoy}"
OUTPUT_PATH="${2:-$ROOT_DIR/assets/myth-henry-aeonvoice.wav}"
VOICE_PROFILE="${VOICE_PROFILE:-Leena}"
CADENCE_MODE="${3:-${CADENCE_MODE:-adaptive}}"

if [[ ! -f "$STORY_PATH" ]]; then
  echo "Story file not found: $STORY_PATH" >&2
  exit 1
fi

echo "Synthesizing narration..."
echo "  story:  $STORY_PATH"
echo "  output: $OUTPUT_PATH"
echo "  voice:  $VOICE_PROFILE"
echo "  cadence:$CADENCE_MODE"

dotnet run --project "$ROOT_DIR/synthesizer/story-audio-synth" -- \
  --input "$STORY_PATH" \
  --output "$OUTPUT_PATH" \
  --voice "$VOICE_PROFILE" \
  --cadence "$CADENCE_MODE"

echo "Done."
