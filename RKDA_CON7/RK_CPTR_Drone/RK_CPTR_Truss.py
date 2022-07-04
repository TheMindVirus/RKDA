import bpy, bmesh, math

def console(data, level = "OUTPUT"):
    for window in bpy.context.window_manager.windows:
        screen = window.screen
        for area in screen.areas:
            if area.type == 'CONSOLE':
                override = {'window': window, 'screen': screen, 'area': area}
                bpy.ops.console.scrollback_append(override, text = str(data), type = level)

def print(data):
    console(data)
                
def error(data):
    console(data, "ERROR")

def radians(degrees):
    return (degrees * math.pi) / 180

def sin(degrees):
    return math.sin(radians(degrees))

def cos(degrees):
    return math.cos(radians(degrees))

def tan(degrees):
    return math.tan(radians(degrees))

def makeTruss(bars, crosses, drawSegments = False):
    verts = []
    edges = []
    for c in range(0, crosses):
        for b in range(0, bars):
            verts.append((sin(((360 * b) / bars) + 45), cos(((360 * b) / bars) + 45), c * 2))
        if drawSegments:
            for b in range(0, bars):
                edges.append(((c * bars) + b, (c * bars) + ((b + 1) % bars)))
        if (c > 0):
            for b in range(0, bars):
                edges.append((((c - 1) * bars) + b, (c * bars) + b))
            for b in range(0, bars):
                edges.append(((c * bars) + ((b + 1) % bars), ((c - 1) * bars) + ((b + 2) % bars)))
    return (verts, edges)

try:
    error("Decoding Encrypted Engram...")
    bpy.context.view_layer.active_layer_collection = bpy.context.view_layer.layer_collection.children[0]
    
    verts, edges = makeTruss(5, 5, True);
    faces = []
    
    mesh = bpy.data.meshes.new("Truss")
    mesh.from_pydata(verts, edges, faces)
    mesh.update()
    
    obj = bpy.data.objects.new("Truss", mesh)
    scene = bpy.context.scene.collection.children[0]
    scene.objects.link(obj)
    obj.select_set(True)
        
    print("Master Rahool is finished.")
except Exception as e:
    error(e)