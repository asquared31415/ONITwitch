#!/bin/bash
echo "[TESTING] Generating test changelogs..."
cargo run --manifest-path scripts/gen_changelog/Cargo.toml -- changelogs/

rm -rf live_docs
git clone git@github.com:asquared31415/asquared31415.github.io.git live_docs
echo "Clearing old docs..."
rm -rf live_docs/twitchintegration/dev_docs/*
echo "Running docfx"
docfx --serve
