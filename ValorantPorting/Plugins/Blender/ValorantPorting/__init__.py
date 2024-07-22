import bpy
import traceback
import zstandard as zstd
import .ValorantRig
from .logger import Log
from .server import ImportServer, MessageServer
from .import_task import ImportTask

bl_info = {
    "name": "Valorant Porting",
    "description": "Valorant Porting Blender Plugin",
    "author": "Half, Zain, DeveloperChipmunk",
    "blender": (4, 0, 0),
    "version": (2, 0, 1),
    "category": "Import",
}


def message_box(message="", title="Message Box", icon='INFO'):
    def draw(self, context):
        self.layout.label(text=message)

    bpy.context.window_manager.popup_menu(draw, title=title, icon=icon)


operators = [
    ValorantRig.RigProperties,
    ValorantRig.SnapIKToFKOperator,
    ValorantRig.ValorantRigPanel,
    ValorantRig.ValorantRigApply,
    ValorantRig.ValorantRigRemove,
    ValorantRig.RigUI,
    ValorantRig.RigLayers
]


def register():
    import io_scene_ueformat
    io_scene_ueformat.zstd_decompresser = zstd.ZstdDecompressor()

    global import_server
    import_server = ImportServer()
    import_server.start()

    def import_server_handler():
        if import_server.has_response():
            try:
                ImportTask().run(import_server.response)
            except Exception as e:
                error_str = str(e)
                Log.error(f"An unhandled error occurred:")
                traceback.print_exc()
                message_box(error_str, "An unhandled error occurred", "ERROR")
            import_server.clear_response()
        return 0.01

    bpy.app.timers.register(import_server_handler, persistent=True)

    for operator in operators:
        bpy.utils.register_class(operator)
    bpy.types.Scene.my_properties = bpy.props.PointerProperty(type=ValorantRig.RigProperties)

    global message_server
    message_server = MessageServer()
    message_server.start()


def unregister():
    import_server.stop()
    message_server.stop()

    for operator in operators:
        bpy.utils.unregister_class(operator)
    del bpy.types.Scene.my_properties


if __name__ == "__main__":
    register()
