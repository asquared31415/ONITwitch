rm -rf live_docs
git clone git@github.com:asquared31415/asquared31415.github.io.git live_docs
echo "Clearing old docs..."
rm -rf live_docs/twitchintegration/dev_docs/*
echo "Running docfx"
docfx
echo "Committing"
cd live_docs
git add .
git commit -m "Twitch Integration docs update" --quiet
COMMIT=$(git log --pretty=format:'%h' -n 1)
echo "Committed $COMMIT"
git push
cd ..
