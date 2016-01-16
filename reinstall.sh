#!/bin/bash

set -ue

MANIFEST_PATH=Assets/Plugins/Android/AndroidManifest.xml

activity=com.unity3d.player.UnityPlayerActivity
root=$( dirname $0 )

devices=$(adb devices | sort | grep device\$ | cut -f1 | tr '\n' ' ')

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
  echo "Extracting activity name from $MANIFEST_PATH"
  activity=$( grep '<activity android:name="' "$MANIFEST_PATH" | cut -d '"' -f 2 )
fi

echo ""
echo "Using:"
echo "- Package identifier: $pkg"
echo "- APK filename      : $apk"
echo "- Android Activity  : $activity"
for device in $devices
do
echo "- Device            : $device"
done
echo

uninstall_pkg()
{
  echo
  echo "$ANDROID_SERIAL Uninstalling $*"
  adb shell pm uninstall $* || true
}

install_pkg()
{
  echo
  echo "$ANDROID_SERIAL adb install $*"
  output=$( adb install $* 2>&1 | grep -v 'KB/s' | grep -v 'pkg:' | grep -v 'Success' )
  [ -z "$output" ]
}

# Begin actual un/re-install and launch
pids=""
device_num=0
for serial in $devices
do
  export ANDROID_SERIAL=$serial
  device_num=$(( $device_num + 1))
  (
    adb shell am start -a android.intent.action.MAIN -c android.intent.category.HOME >/dev/null
    #user_count=$(( $( adb shell pm list users | grep UserInfo | wc -l ) ))
    #user=$( adb shell pm list users | grep UserInfo | awk "NR == $device_num % ($user_count + 1)" | sed -E 's/.*UserInfo.([0-9]+).*/\1/' )
    adb_args=$( cat android-serial.txt | grep $ANDROID_SERIAL | cut -d' ' -f2- )
    install_pkg -r -g $adb_args $apk ||
    (
      echo " and reinstalling on $serial"
      uninstall_pkg $pkg \
       && install_pkg $adb_args $apk
    )
    echo
    echo "$ANDROID_SERIAL Launching $pkg/$activity"
    adb shell am start $adb_args -n $pkg/$activity | grep -v 'Starting: Intent'
  ) &
  pids="$pids $!"
done

for pid in $pids
do
  wait $pid
done
