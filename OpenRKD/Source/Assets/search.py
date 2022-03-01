import os

#lookfor = "LEGO Data"
lookfor = "LEGO Tools"
#lookfor = "Version.asset"

extensions= \
[
    ".cs",
]

matches = 0
for root, dirs, files in os.walk(os.getcwd()):
    for filename in files:
        name, ext = os.path.splitext(filename)
        filepath = os.path.join(root, filename)
        relpath = os.path.relpath(filepath)
        if len(extensions) > 0 and ext in extensions:
            file = open(filepath, "r")
            data = file.read()
            file.close()
            lines = data.split("\n")
            for line in range(0, len(lines)):
                if lookfor in lines[line]:
                    print(relpath + " at line " + str(line + 1))
                    matches += 1
print(str(matches) + " Matching Files for \"" + lookfor + "\"")
