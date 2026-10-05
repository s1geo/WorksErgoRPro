import os
import subprocess
import glob
import re
import json

chm_dir = r"D:\Apps\RProv222\Help"
target_dir = r"D:\Git\WorksErgoRPro\knowledge_base\03_cad_documentation_and_help"

chm_files = [
    "Help_Ergonomics_EN.chm",
    "Help_Ergonomics_RU.chm",
    "Help_Ergonomics_WPP_EN.chm",
    "Help_Ergonomics_WPP_RU.chm",
    "Help_MoCap_EN.chm",
    "Help_MoCap_RU.chm",
    "Python_API.chm"
]

topics_index = []

for chm_name in chm_files:
    chm_path = os.path.join(chm_dir, chm_name)
    out_folder = os.path.join(target_dir, os.path.splitext(chm_name)[0])
    os.makedirs(out_folder, exist_ok=True)
    
    print(f"Decompiling {chm_name} -> {out_folder}")
    cmd = f'hh.exe -decompile "{out_folder}" "{chm_path}"'
    subprocess.run(cmd, shell=True)
    
    # Scan extracted HTML files for titles and headers
    html_files = glob.glob(os.path.join(out_folder, "**", "*.htm*"), recursive=True)
    print(f"Extracted {len(html_files)} HTML pages from {chm_name}")
    
    for hf in html_files:
        rel_path = os.path.relpath(hf, target_dir).replace("\\", "/")
        title = os.path.basename(hf)
        snippet = ""
        try:
            with open(hf, "r", encoding="utf-8", errors="ignore") as f:
                content = f.read()
                m_t = re.search(r"<title>(.*?)</title>", content, re.IGNORECASE)
                if m_t:
                    title = m_t.group(1).strip()
                # Clean html tags for snippet
                clean_text = re.sub(r"<[^>]+>", " ", content)
                clean_text = " ".join(clean_text.split())
                snippet = clean_text[:300]
        except Exception as e:
            snippet = f"Error: {e}"
            
        topics_index.append({
            "source_chm": chm_name,
            "title": title,
            "rel_path": rel_path,
            "snippet": snippet
        })

index_json_path = os.path.join(target_dir, "cad_help_index.json")
with open(index_json_path, "w", encoding="utf-8") as f:
    json.dump(topics_index, f, ensure_ascii=False, indent=2)

print(f"\nIndexed total {len(topics_index)} documentation topics in {index_json_path}")
