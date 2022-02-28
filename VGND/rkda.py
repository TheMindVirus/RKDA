import bpy, bmesh, math, random

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

def Randomise(n = 100, s = 1):
    r = []
    for i in range(0, n):
        r.append(random.random() * s)
    return r

def Block(blocks = 100, spacing = 10):
    verts = []
    edges = []
    faces = []
    x = [ 1,  1, -1, -1,  1, -1, -1,  1]
    y = [ 1, -1, -1,  1,  1,  1, -1, -1]
    z = [ 1,  1,  1,  1, -1, -1, -1, -1]
    t = \
    [
        (0, 1), (1, 2), (2, 3), (3, 0),
        (4, 5), (5, 6), (6, 7), (7, 4),
        (0, 4), (1, 7), (2, 6), (3, 5),
    ]
    ie = \
    [
        (0, 1, 2, 3),
        (0, 4, 7, 1),
        (1, 7, 6, 2),
        (2, 6, 5, 3),
        (3, 5, 4, 0),
        (4 ,5, 6, 7),
    ]
    for i in range(0, blocks):
        r = Randomise(s = spacing)
        cx, cy, cz = r[0:3]
        for v in range(0, 8):
            verts.append((cx + x[v], cy + y[v], cz + z[v]))
        for e in range(0, len(t)):
            ci = i * 8
            edges.append((ci + t[e][0], ci + t[e][1]))
        for f in range(0, len(ie)):
            ch = i * 8
            faces.append(
            (
                ch + ie[f][0],
                ch + ie[f][1],
                ch + ie[f][2],
                ch + ie[f][3])
            )
    return (verts, edges, faces)

def Road(roads = 100, distance = 10):
    verts = []
    edges = []
    faces = []
    il = \
    [
        ( 1.0,  1.0,  0.0),
        ( 1.0, -1.0,  0.0),
        (-1.0, -1.0,  0.0),
        (-1.0,  1.0,  0.0),
        ( 1.0,  0.8,  0.2),
        (-1.0,  0.8,  0.2),
        (-1.0, -0.8,  0.2),
        ( 1.0, -0.8,  0.2),
    ]
    im = \
    [
        (0, 1), (1, 2), (2, 3), (3, 0),
        (4, 5), (6, 7),
        (0, 4), (1, 7), (2, 6), (3, 5),
    ]
    ie = \
    [
        (0, 1, 2, 3),
        (4, 5, 3, 0),
        (6, 7, 1, 2),
    ]
    r = Randomise(s = distance)
    ll = len(il)
    lm = len(im)
    le = len(ie)
    cx, cy, cz = (-distance, r[0], 0)
    for i in range(0, distance):
        ch = i * ll
        for v in range(0, ll):
            verts.append(
            (
                cx + il[v][0],
                cy + il[v][1],
                cz + il[v][2])
            )
        for e in range(0, lm):
            edges.append(
            (
                ch + im[e][0],
                ch + im[e][1])
            )
        for f in range(0, le):
            faces.append(
            (
                ch + ie[f][0],
                ch + ie[f][1],
                ch + ie[f][2],
                ch + ie[f][3])
            )
        d = Randomise(s = distance)
        for ir in range(0, len(r)):
            r[ir] += d[ir]
            r[ir] /= 2.0
        cx += 2.0
        cy = r[0]
        cz = 0
    return (verts, edges, faces)

def make(name, data):
    mesh = bpy.data.meshes.new(name)
    mesh.from_pydata(*data)
    mesh.update()
    
    obj = bpy.data.objects.new(name, mesh)
    scene = bpy.context.scene.collection.children[0]
    scene.objects.link(obj)
    obj.select_set(True)

try:
    error("Decoding Encrypted Engram...")
    random.seed("ABC123")
    bpy.context.view_layer.active_layer_collection = bpy.context.view_layer.layer_collection.children[0]
    
    #make("Block", Block())
    make("Road", Road())
        
    print("Master Rahool is finished.")
except Exception as e:
    error(e)