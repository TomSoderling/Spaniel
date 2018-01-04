#!/usr/bin/env bash

# Insert the iOS App Center Secret into ApiKeys.cs file

set -e # Exit immediately if a command exits with a non-zero status (failure)

filename="$PWD/../Spaniel/ApiKeys.cs" # target file

echo "*****************************************************************************"
echo "App Center Secret Inserter"
echo "*****************************************************************************"
echo "        Working directory:" $PWD
echo "Secret from env variables:" $AppCenterSecretiOS # this is set up in the App Center build config
echo "              Target file:" $filename
echo "*****************************************************************************"
echo ""


# Check if file exists first
if [ -e $filename ]
then
    echo "Target file found"
else
    echo "Target file not found. Exiting."
    exit 1 # exit with unspecified error code. Should be obvious why we can't continue the script
fi


# Load the file
echo "Load file: $filename"
apiKeysFile=$(<$filename)


# Seach for replacement text in file
stringToFind="\[your iOS App Center secret goes here\]"
matchFound=false # flag to indicate we found a match

while IFS= read -r line; do
if [[ $line == *$stringToFind* ]]
then
    echo "Line found:" $line
    matchFound=true

    # Edit the file and replace the found text with the Secret text
    # sed: stream editior
    #  -i: in-place edit
    #  -e: the following string is an instruction or set of instructions
    #   s: substitute pattern2 ($AppCenterSecretiOS) for first instance of pattern1 ($stringToFind) in a line
    cat $filename | sed -i -e "s/$stringToFind/$AppCenterSecretiOS/" $filename

    echo "App secret inserted"

    break # found the line, so break out of loop
fi
done < "$filename"

# Show error if match not found
if [ $matchFound == false ]
then
    echo "Unable to find match for:" $stringToFind
    exit 1 # exit with unspecified error code.
fi

echo "*****************************************************************************"
echo "Script complete"
echo "*****************************************************************************"