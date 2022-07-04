from PIL import Image
import random, math

W = 512
H = 512

img = Image.new("RGBA", (W, H))

def radius(x, y, w, h):
    w2 = (w / 2)
    h2 = (h / 2)
    x -= w2 - 0.5
    y -= h2 - 0.5
    return math.sqrt(pow(x, 2) + pow(y, 2))# / math.sqrt(pow(w2, 2) + pow(h2, 2))

def tension(v, vmax):
    return v / (vmax - v) * 10.0

for y in range(0, H):
    for x in range(0, W):
        r2 = radius(x, y, W, H)
        d2 = 256.0
        r = 255 
        g = 0
        b = 64
        a = int(tension(r2, d2)) if r2 < d2 else 0
        img.putpixel((x, y), (r, g, b, a))

img.save("RK_CPTR_Ring.png")
print("Done!")
