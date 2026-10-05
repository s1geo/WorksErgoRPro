import os
import sys
import zipfile
import struct
import shutil
import uuid

def create_tga_icon(width, height, r, g, b):
    # 24-bit uncompressed Truevision TGA
    hdr = struct.pack('<BBBHHBHHHHBB', 0, 0, 2, 0, 0, 0, 0, 0, width, height, 24, 0x20)
    # Simple border / visual pattern for barrier icon
    pixels = bytearray()
    for y in range(height):
        for x in range(width):
            if x < 4 or x >= width - 4 or y < 4 or y >= height - 4:
                # Dark gray border
                pixels.extend([40, 40, 40])
            elif (x + y) % 16 < 8:
                # Hazard yellow/orange
                pixels.extend([b, g, r])
            else:
                # Hazard dark stripe
                pixels.extend([20, 20, 20])
    return hdr + bytes(pixels)

def build_barrier_rpro(output_path):
    os.makedirs(os.path.dirname(output_path), exist_ok=True)
    comp_vcid = str(uuid.uuid4())

    model_xml = f'''<?xml version="1.0" encoding="utf-8"?>
<VcModel xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" version="1" xmlns="http://schemas.RProSoftDigital1.com/2017/01/component/componentxml">
  <Properties>
    <Property name="VCID">{comp_vcid}</Property>
    <Property name="ModelType">Component</Property>
    <Property name="Name">Barrier</Property>
    <Property name="Description">Works Ergo Parametric Collision Barrier and Safety Obstacle for workstation reach and clearance audits.</Property>
    <Property name="Type">Ergonomics</Property>
    <Property name="Manufacturer">R-Pro WorksErgo</Property>
    <Property name="Author">R-Pro Engineering Ecosystem</Property>
    <Property name="BarrierLength">1000.0</Property>
    <Property name="BarrierWidth">100.0</Property>
    <Property name="BarrierHeight">1100.0</Property>
  </Properties>
  <ModelUrl>component.rsc</ModelUrl>
  <ThumbnailImageUrl>layout_icon.tga</ThumbnailImageUrl>
  <PreviewImageUrl>component_icon_preview.tga</PreviewImageUrl>
</VcModel>
'''

    component_dat = f'''VcId "{comp_vcid}"
Revision 1
DetailedRevision "1.0.0.0"
Tags "Ergonomics;Barrier;Obstacle;Safety;Clearance"
KeywordMap
{{
  Keyword
  {{
    Key "Type"
    Value "Ergonomics"
  }}
  Keyword
  {{
    Key "Author"
    Value "R-Pro WorksErgo"
  }}
  Keyword
  {{
    Key "Manufacturer"
    Value "R-Pro"
  }}
}}
Items
{{
  Item
  {{
    Name "Barrier"
    Icon "layout_icon.tga"
    PreviewIcon "component_icon_preview.tga"
    Description "Works Ergo Parametric Collision Barrier and Safety Obstacle"
    Logo ""
    Uri "component.rsc"
  }}
}}
'''

    materials_dat = '''Materials
{
  Material
  {
    Name "Barrier_Hazard"
    Diffuse 0.95 0.55 0.15 0.7
    Emissive 0.1 0.05 0.0 1.0
    Specular 0.8 0.8 0.8 1.0
    Ambient 0.3 0.3 0.3 1.0
    Shininess 0.6
    Transparency 0.3
  }
}
'''

    component_rsc = f'''VCMD0028040000000000COMPONENT          
Node "rSimResource"
{{
Name "Barrier"
Id 1
NodeClass 
{{
Id 1
Feature "rTransformFeature"
{{
Name "TGeo_Barrier"

Visible 1
Transform 
{{
  Expression "Tx(-0.5*BarrierLength).Ty(-0.5*BarrierWidth).Tz(0)"
}}

Feature "rPrimitiveBoxFeature"
{{
Name "Barrier_Box"

Visible 1
Length 
{{
  Expression "BarrierLength"
}}

Width 
{{
  Expression "BarrierWidth"
}}

Height 
{{
  Expression "BarrierHeight"
}}

}}
}}
SimAttribute "rSimApplyMaterialAttribute"
{{
Material  "Barrier_Hazard"
{{
  Ambient
  {{
    Red 0.3
    Green 0.3
    Blue 0.3
  }}
  Diffuse
  {{
    Red 0.95
    Green 0.55
    Blue 0.15
  }}
  Specular
  {{
    Color
    {{
      Red 0.8
      Green 0.8
      Blue 0.8
    }}
    Shininess 0.6
  }}
}}

}}
VCID "{comp_vcid}"
}}
VCID {comp_vcid}
Revision 1
Location 1 0 0 0 0 1 0 0 0 0 1 0 0 0 0 1 
ActiveSimulationLevel detailed
BOM  0

BOMname  "Barrier"

BOMdescription  "Works Ergo Parametric Collision Barrier and Safety Obstacle"

Category  "Ergonomics"

VariableSpace ""
{{
  Variable "rTVariable<rBool>"
  {{
    Name "Visible"
    Value 1
    Group -1130
    Settings
    {{
      VISIBLE
      EDITABLE_DISCONNECTED
      EDITABLE_CONNECTED
      EDITABLE_SIMULATING
      MANAGED
    }}
  }}
  Variable "rTVariable<rDouble>"
  {{
    Name "BarrierLength"
    Value 1000
    Group 65536
    Settings
    {{
      VISIBLE
      EDITABLE_DISCONNECTED
      EDITABLE_CONNECTED
      EDITABLE_SIMULATING
      ON_EDIT_REBUILD
    }}
    Quantity "Distance"
    Magnitude 1
  }}
  Variable "rTVariable<rDouble>"
  {{
    Name "BarrierWidth"
    Value 100
    Group 65537
    Settings
    {{
      VISIBLE
      EDITABLE_DISCONNECTED
      EDITABLE_CONNECTED
      EDITABLE_SIMULATING
      ON_EDIT_REBUILD
    }}
    Quantity "Distance"
    Magnitude 1
  }}
  Variable "rTVariable<rDouble>"
  {{
    Name "BarrierHeight"
    Value 1100
    Group 65538
    Settings
    {{
      VISIBLE
      EDITABLE_DISCONNECTED
      EDITABLE_CONNECTED
      EDITABLE_SIMULATING
      ON_EDIT_REBUILD
    }}
    Quantity "Distance"
    Magnitude 1
  }}
}}
}}
'''

    icon_data = create_tga_icon(128, 128, 240, 140, 30)

    # Package into ZIP (.rpro)
    with zipfile.ZipFile(output_path, 'w', compression=zipfile.ZIP_DEFLATED) as z:
        z.writestr('model.xml', model_xml.encode('utf-8'))
        z.writestr('component.dat', component_dat.encode('utf-8'))
        z.writestr('component.rsc', component_rsc.encode('utf-8'))
        z.writestr('materials.dat', materials_dat.encode('utf-8'))
        z.writestr('component_icon_preview.tga', icon_data)
        z.writestr('layout_icon.tga', icon_data)

    print(f"Successfully generated: {output_path} ({os.path.getsize(output_path)} bytes)")

if __name__ == '__main__':
    repo_target = os.path.abspath(r'D:\Git\WorksErgoRPro\components\Barrier.rpro')
    my_models_target = os.path.abspath(r'C:\Users\Jojo\Documents\R-Pro\0.2\My Models\WorksErgo\Barrier.rpro')

    build_barrier_rpro(repo_target)
    os.makedirs(os.path.dirname(my_models_target), exist_ok=True)
    shutil.copy2(repo_target, my_models_target)
    print(f"Deployed to R-Pro eCatalog: {my_models_target}")
