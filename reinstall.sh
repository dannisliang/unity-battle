#!/bin/bash

set -ue

MANIFEST_PATH=Assets/Plugins/Android/AndroidManifest.xml

activity=com.unity3d.player.UnityPlayerActivity
root=$( dirname $0 )

devices=$(adb devices | grep device\$ | cut -f1 | tr '\n' ' ')

# Determine Android package name
pkg=$( grep bundleIdentifier ProjectSettings/ProjectSettings.asset | sed 's/ //g' | cut -d: -f2 )

# Determine APK filename
apk="$pkg.apk"
if [ ! -r "$pkg.apk" ]
then
  apk=$( ls -1 "$root"/*.apk )
  apk_count=$(( $( echo "$apk" | wc -l ) ))
  if [ $apk_count -ne 1 ]
  then
    echo "ERROR: Found $apk_count APKs but expecting exactly 1" 1>&2
    ls -l $apk 1>&2
    exit 1
  fi
fi

# Determine overriding Android activity name from AndroidManifest.xml
if [ -f "$MANIFEST_PATH" ]
then
  echo
  echo "Extracting activity name from $MANIFEST_PATH …"
  activity=$( grep '<activity android:name="' "$MANIFEST_PATH" | cut -d '"' -f 2 )
fi

echo ""
echo "Using:"
echo "- Package identifier: $pkg"
echo "- APK filename      : $apk"
echo "- Android Activity  : $activity"
echo "- Device(s)         : $devices"
echo

uninstall_pkg()
{
  echo
  echo "$ANDROID_SERIAL Uninstalling $1 …"
  adb shell pm uninstall $1 || true
}

install_pkg()
{
  echo
  echo "$ANDROID_SERIAL Installing $1 …"
  adb install -r --user 0 $1
}

# Begin actual un/re-install and launch
pids=""
for serial in $devices
do
  (
    export ANDROID_SERIAL=$serial
    install_pkg $apk ||
    (
      echo " and reinstalling on $serial …"
      uninstall_pkg $pkg \
       && install_pkg $apk
    )
    echo
    echo "$ANDROID_SERIAL Launching $pkg/$activity …"
    adb shell am start -n $pkg/$activity
  ) &
  pids="$pids $!"
done

for pid in $pids
do
  wait $pid
done
