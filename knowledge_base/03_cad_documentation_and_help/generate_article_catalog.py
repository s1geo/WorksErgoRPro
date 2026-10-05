import os
import glob
import re
import json

base_dir = r"D:\Git\WorksErgoRPro\knowledge_base\03_cad_documentation_and_help\rpro_help_decompiled"
output_file = r"D:\Git\WorksErgoRPro\knowledge_base\03_cad_documentation_and_help\rpro_articles.json"

doc_sources = [
    {
        "folder": "Help_RP_ru",
        "category": "R-Pro CAD Core Manual",
        "language": "RU",
        "product": "R-Pro",
        "encoding": "windows-1251"
    },
    {
        "folder": "Help_RP",
        "category": "R-Pro / VC CAD Core Manual",
        "language": "EN",
        "product": "R-Pro / Visual Components",
        "encoding": "utf-8"
    },
    {
        "folder": "Help_Ergonomics_RU",
        "category": "Ergonomics Module (RULA/REBA/ISO)",
        "language": "RU",
        "product": "R-Pro",
        "encoding": "windows-1251"
    },
    {
        "folder": "Help_Ergonomics_EN",
        "category": "Ergonomics Module (RULA/REBA/ISO)",
        "language": "EN",
        "product": "R-Pro",
        "encoding": "utf-8"
    },
    {
        "folder": "Help_Ergonomics_WPP_RU",
        "category": "Working Postures Module (WPP)",
        "language": "RU",
        "product": "R-Pro",
        "encoding": "windows-1251"
    },
    {
        "folder": "Help_Ergonomics_WPP_EN",
        "category": "Working Postures Module (WPP)",
        "language": "EN",
        "product": "R-Pro",
        "encoding": "utf-8"
    },
    {
        "folder": "Help_MoCap_RU",
        "category": "Motion Capture Module (MoCap)",
        "language": "RU",
        "product": "R-Pro",
        "encoding": "windows-1251"
    },
    {
        "folder": "Help_MoCap_EN",
        "category": "Motion Capture Module (MoCap)",
        "language": "EN",
        "product": "R-Pro",
        "encoding": "utf-8"
    },
    {
        "folder": "Python_API",
        "category": "Python API Reference",
        "language": "EN",
        "product": "Visual Components / R-Pro",
        "encoding": "utf-8"
    }
]

catalog = []

for src in doc_sources:
    folder_path = os.path.join(base_dir, src["folder"])
    if not os.path.isdir(folder_path):
        print(f"Directory not found: {folder_path}")
        continue

    html_files = glob.glob(os.path.join(folder_path, "**", "*.htm*"), recursive=True)
    print(f"Processing {src['folder']}: found {len(html_files)} files...")

    for fpath in html_files:
        rel_path = os.path.relpath(fpath, os.path.dirname(base_dir)).replace("\\", "/")
        title = os.path.splitext(os.path.basename(fpath))[0]
        snippet = ""
        keywords = []

        encodings_to_try = [src["encoding"], "utf-8", "windows-1251", "latin-1"]
        content = ""
        for enc in encodings_to_try:
            try:
                with open(fpath, "r", encoding=enc) as f:
                    content = f.read()
                break
            except Exception:
                continue

        if content:
            # Extract title
            m_t = re.search(r"<title>(.*?)</title>", content, re.IGNORECASE | re.DOTALL)
            if m_t:
                extracted_t = m_t.group(1).strip()
                if extracted_t and not extracted_t.startswith("Ã") and len(extracted_t) > 1:
                    title = " ".join(extracted_t.split())

            # Fallback to h1/h2 if title is generic
            if title in ["", "Untitled", "New Topic", "Introduction", os.path.splitext(os.path.basename(fpath))[0]]:
                m_h = re.search(r"<h[1-2][^>]*>(.*?)</h[1-2]>", content, re.IGNORECASE | re.DOTALL)
                if m_h:
                    cand = " ".join(re.sub(r"<[^>]+>", "", m_h.group(1)).split()).strip()
                    if len(cand) > 2:
                        title = cand

            # Extract meta keywords
            m_kw = re.search(r'<meta\s+name=["\']keywords["\']\s+content=["\'](.*?)["\']', content, re.IGNORECASE)
            if m_kw:
                kw_str = m_kw.group(1).strip()
                if kw_str:
                    keywords = [k.strip() for k in kw_str.split(",") if k.strip()]

            # Extract snippet
            clean_text = re.sub(r"<script.*?>.*?</script>", " ", content, flags=re.IGNORECASE | re.DOTALL)
            clean_text = re.sub(r"<style.*?>.*?</style>", " ", clean_text, flags=re.IGNORECASE | re.DOTALL)
            clean_text = re.sub(r"<[^>]+>", " ", clean_text)
            clean_text = " ".join(clean_text.split())
            snippet = clean_text[:300]

        catalog.append({
            "title": title,
            "category": src["category"],
            "language": src["language"],
            "product": src["product"],
            "source_folder": src["folder"],
            "relative_path": rel_path,
            "keywords": keywords,
            "snippet": snippet
        })

print(f"\nTotal catalog entries: {len(catalog)}")
with open(output_file, "w", encoding="utf-8") as f:
    json.dump(catalog, f, ensure_ascii=False, indent=2)

print(f"Catalog saved to {output_file}")
