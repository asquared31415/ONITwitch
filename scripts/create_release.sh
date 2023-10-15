if [ $# -lt 1 ]
then
    echo "Enter the version of the release"
    exit 1
fi

VERSION="$1"
TARGET_DIR="releases/$VERSION"

if [ -d $TARGET_DIR ]
then
    echo "Directory $TARGET_DIR already exists"
    exit 1
fi

mkdir -p $TARGET_DIR

cp -r obj/ONITwitch/ $TARGET_DIR
cp -r obj/ONITwitchLib/* $TARGET_DIR

echo "Release $VERSION prepared"
