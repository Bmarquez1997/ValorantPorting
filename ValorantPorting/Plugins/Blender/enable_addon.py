import bpy
import sys


def main():
    if "ValorantPorting" in bpy.context.preferences.addons:
        print("ValorantPorting Addon Already Enabled, Exiting")
        return

    print("Enabling ValorantPorting Addon")
    bpy.ops.preferences.addon_enable(module='ValorantPorting')
    bpy.ops.wm.save_userpref()
    print("Saved Userprefs")


main()
sys.exit(0)
