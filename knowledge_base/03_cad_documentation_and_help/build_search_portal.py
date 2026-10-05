import os
import glob
import re
import json

base_dir = r"D:\Git\WorksErgoRPro\knowledge_base\03_cad_documentation_and_help"

sections = [
    ("Help_Ergonomics_RU", "Р-Про: Модуль «Эргономика» (Базовый)"),
    ("Help_Ergonomics_WPP_RU", "Р-Про: Модуль «Рабочие позы» (WPP)"),
    ("Help_MoCap_RU", "Р-Про: Захват движения (MoCap)"),
    ("Python_API", "Р-Про / Visual Components: Python API Справочник"),
    ("Help_RP_RU", "Р-Про: Полное руководство пользователя CAD"),
    ("Temp_Help_Ergonomics_v0.1.html", "Work(s) Ergo: Руководство взаимодействия в 3D")
]

all_articles = []

for folder_or_file, cat_title in sections:
    full_path = os.path.join(base_dir, folder_or_file)
    if os.path.isfile(full_path):
        # Single HTML file
        try:
            with open(full_path, "r", encoding="utf-8", errors="ignore") as f:
                content = f.read()
            m_t = re.search(r"<title>(.*?)</title>", content, re.IGNORECASE)
            title = m_t.group(1).strip() if m_t else folder_or_file
            clean_text = " ".join(re.sub(r"<[^>]+>", " ", content).split())
            all_articles.append({
                "category": cat_title,
                "title": title,
                "url": folder_or_file,
                "snippet": clean_text[:280]
            })
        except Exception as e:
            pass
    elif os.path.isdir(full_path):
        html_files = glob.glob(os.path.join(full_path, "**", "*.htm*"), recursive=True)
        for hf in html_files:
            rel_url = os.path.relpath(hf, base_dir).replace("\\", "/")
            try:
                with open(hf, "r", encoding="utf-8", errors="ignore") as f:
                    content = f.read()
                m_t = re.search(r"<title>(.*?)</title>", content, re.IGNORECASE)
                if not m_t:
                    m_t = re.search(r"<h[1-2][^>]*>(.*?)</h[1-2]>", content, re.IGNORECASE)
                title = m_t.group(1).strip() if m_t else os.path.splitext(os.path.basename(hf))[0]
                title = " ".join(re.sub(r"<[^>]+>", "", title).split())
                if not title or len(title) < 2:
                    title = os.path.basename(hf)
                clean_text = " ".join(re.sub(r"<[^>]+>", " ", content).split())
                all_articles.append({
                    "category": cat_title,
                    "title": title,
                    "url": rel_url,
                    "snippet": clean_text[:280]
                })
            except Exception as e:
                pass

print(f"Total indexed articles: {len(all_articles)}")

# Generate an offline, reactive HTML Search Portal
portal_html = f"""<!DOCTYPE html>
<html lang="ru">
<head>
<meta charset="UTF-8">
<meta name="viewport" content="width=device-width, initial-scale=1.0">
<title>Портал Документации Р-Про, Visual Components и Work(s) Ergo</title>
<style>
:root {{
  --bg-primary: #0f172a;
  --bg-secondary: #1e293b;
  --bg-card: #334155;
  --text-main: #f8fafc;
  --text-muted: #94a3b8;
  --accent: #38bdf8;
  --accent-hover: #0284c7;
  --border: #475569;
  --tag-bg: #0369a1;
}}
* {{ box-sizing: border-box; margin: 0; padding: 0; font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Helvetica, Arial, sans-serif; }}
body {{ background: var(--bg-primary); color: var(--text-main); padding: 24px; min-height: 100vh; }}
.header {{ max-width: 1200px; margin: 0 auto 24px; text-align: center; }}
.header h1 {{ font-size: 28px; margin-bottom: 8px; color: var(--accent); }}
.header p {{ color: var(--text-muted); font-size: 15px; }}
.search-container {{ max-width: 1200px; margin: 0 auto 24px; display: flex; gap: 12px; }}
#searchInput {{
  flex: 1; padding: 14px 20px; font-size: 16px; border-radius: 8px;
  border: 1px solid var(--border); background: var(--bg-secondary); color: #fff; outline: none;
}}
#searchInput:focus {{ border-color: var(--accent); box-shadow: 0 0 0 3px rgba(56,189,248,0.25); }}
.filter-tabs {{ max-width: 1200px; margin: 0 auto 24px; display: flex; gap: 8px; flex-wrap: wrap; }}
.tab-btn {{
  background: var(--bg-secondary); border: 1px solid var(--border); color: var(--text-muted);
  padding: 8px 16px; border-radius: 6px; cursor: pointer; font-size: 13px; font-weight: 500; transition: all 0.2s;
}}
.tab-btn:hover, .tab-btn.active {{ background: var(--tag-bg); color: #fff; border-color: var(--accent); }}
.stats {{ max-width: 1200px; margin: 0 auto 12px; color: var(--text-muted); font-size: 14px; }}
.results-grid {{
  max-width: 1200px; margin: 0 auto; display: grid;
  grid-template-columns: repeat(auto-fill, minmax(360px, 1fr)); gap: 16px;
}}
.card {{
  background: var(--bg-secondary); border: 1px solid var(--border); border-radius: 8px;
  padding: 16px; display: flex; flex-direction: column; justify-content: space-between;
  transition: transform 0.15s, border-color 0.15s;
}}
.card:hover {{ transform: translateY(-2px); border-color: var(--accent); }}
.card-category {{
  font-size: 11px; text-transform: uppercase; letter-spacing: 0.5px;
  color: var(--accent); margin-bottom: 6px; font-weight: 700;
}}
.card-title {{ font-size: 16px; font-weight: 600; margin-bottom: 8px; color: #fff; }}
.card-snippet {{ font-size: 13px; color: var(--text-muted); line-height: 1.5; margin-bottom: 12px; flex: 1; }}
.card-link {{
  display: inline-block; background: var(--tag-bg); color: #fff; text-decoration: none;
  padding: 8px 12px; border-radius: 6px; font-size: 13px; text-align: center; font-weight: 500;
  transition: background 0.2s;
}}
.card-link:hover {{ background: var(--accent-hover); }}
</style>
</head>
<body>

<div class="header">
  <h1>База Знаний и Справочник Р-Про & Work(s) Ergo</h1>
  <p>Локальный поисковый портал по всем 600+ темам официальной документации без расхода токенов</p>
</div>

<div class="search-container">
  <input type="text" id="searchInput" placeholder="Поиск по API, классам (например, vcComponent, Snapping, L5/S1, RULA, Servo)..." autofocus>
</div>

<div class="filter-tabs" id="filterTabs">
  <button class="tab-btn active" data-cat="all">Все разделы</button>
  <button class="tab-btn" data-cat="Python_API">Python API (Visual Components)</button>
  <button class="tab-btn" data-cat="Help_Ergonomics_RU">Эргономика Р-Про</button>
  <button class="tab-btn" data-cat="Help_Ergonomics_WPP_RU">Рабочие позы (WPP)</button>
  <button class="tab-btn" data-cat="Help_MoCap_RU">Захват движения (MoCap)</button>
  <button class="tab-btn" data-cat="Help_RP_RU">Руководство CAD Р-Про</button>
  <button class="tab-btn" data-cat="Work(s) Ergo">Work(s) Ergo 3D Guide</button>
</div>

<div class="stats" id="stats">Загрузка индекса...</div>
<div class="results-grid" id="resultsGrid"></div>

<script>
const articles = {json.dumps(all_articles, ensure_ascii=False)};
let currentCat = "all";

const searchInput = document.getElementById("searchInput");
const resultsGrid = document.getElementById("resultsGrid");
const stats = document.getElementById("stats");
const filterTabs = document.getElementById("filterTabs");

function render() {{
  const query = searchInput.value.toLowerCase().trim();
  const filtered = articles.filter(a => {{
    const matchCat = (currentCat === "all") || a.category.includes(currentCat);
    if (!matchCat) return false;
    if (!query) return true;
    return a.title.toLowerCase().includes(query) || a.snippet.toLowerCase().includes(query);
  }});

  stats.textContent = `Найдено документов: ${{filtered.length}} из ${{articles.length}}`;
  resultsGrid.innerHTML = filtered.slice(0, 100).map(a => `
    <div class="card">
      <div>
        <div class="card-category">${{a.category}}</div>
        <div class="card-title">${{a.title}}</div>
        <div class="card-snippet">${{a.snippet}}</div>
      </div>
      <a class="card-link" href="${{a.url}}" target="_blank">Открыть документ &rarr;</a>
    </div>
  `).join("");
}}

searchInput.addEventListener("input", render);

filterTabs.addEventListener("click", e => {{
  if (e.target.classList.contains("tab-btn")) {{
    document.querySelectorAll(".tab-btn").forEach(b => b.classList.remove("active"));
    e.target.classList.add("active");
    currentCat = e.target.dataset.cat;
    render();
  }}
}});

render();
</script>
</body>
</html>
"""

portal_path = os.path.join(base_dir, "documentation_portal.html")
with open(portal_path, "w", encoding="utf-8") as f:
    f.write(portal_html)

print(f"Created documentation search portal at: {portal_path}")
