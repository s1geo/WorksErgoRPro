import os
import re
import json

base_dir = r"D:\Git\WorksErgoRPro\knowledge_base\03_cad_documentation_and_help"
json_path = os.path.join(base_dir, "rpro_articles.json")

with open(json_path, "r", encoding="utf-8") as f:
    catalog_items = json.load(f)

# Also check for Work(s) Ergo Guide Temp_Help_Ergonomics_v0.1.html if present
worksergo_guide = os.path.join(base_dir, "Temp_Help_Ergonomics_v0.1.html")
if os.path.isfile(worksergo_guide):
    try:
        with open(worksergo_guide, "r", encoding="utf-8", errors="ignore") as f:
            c = f.read()
        m_t = re.search(r"<title>(.*?)</title>", c, re.IGNORECASE)
        t = m_t.group(1).strip() if m_t else "Work(s) Ergo Guide"
        clean = " ".join(re.sub(r"<[^>]+>", " ", c).split())
        catalog_items.append({
            "title": t,
            "category": "Work(s) Ergo: 3D Guide",
            "language": "RU",
            "product": "WorksErgo",
            "source_folder": "",
            "relative_path": "Temp_Help_Ergonomics_v0.1.html",
            "keywords": ["Work(s) Ergo", "DHM", "3D scene", "Posing", "Cylinders"],
            "snippet": clean[:300]
        })
    except Exception:
        pass

print(f"Total articles to render in portal: {len(catalog_items)}")

portal_articles = []
for item in catalog_items:
    portal_articles.append({
        "category": item["category"],
        "title": item["title"],
        "url": item["relative_path"],
        "lang": item["language"],
        "prod": item["product"],
        "snippet": item["snippet"]
    })

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
  --tag-lang: #059669;
}}
* {{ box-sizing: border-box; margin: 0; padding: 0; font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Helvetica, Arial, sans-serif; }}
body {{ background: var(--bg-primary); color: var(--text-main); padding: 24px; min-height: 100vh; }}
.header {{ max-width: 1300px; margin: 0 auto 24px; text-align: center; }}
.header h1 {{ font-size: 28px; margin-bottom: 8px; color: var(--accent); }}
.header p {{ color: var(--text-muted); font-size: 15px; }}
.search-container {{ max-width: 1300px; margin: 0 auto 20px; display: flex; gap: 12px; }}
#searchInput {{
  flex: 1; padding: 14px 20px; font-size: 16px; border-radius: 8px;
  border: 1px solid var(--border); background: var(--bg-secondary); color: #fff; outline: none;
}}
#searchInput:focus {{ border-color: var(--accent); box-shadow: 0 0 0 3px rgba(56,189,248,0.25); }}
.filter-tabs {{ max-width: 1300px; margin: 0 auto 20px; display: flex; gap: 8px; flex-wrap: wrap; }}
.tab-btn {{
  background: var(--bg-secondary); border: 1px solid var(--border); color: var(--text-muted);
  padding: 8px 14px; border-radius: 6px; cursor: pointer; font-size: 13px; font-weight: 500; transition: all 0.2s;
}}
.tab-btn:hover, .tab-btn.active {{ background: var(--tag-bg); color: #fff; border-color: var(--accent); }}
.stats {{ max-width: 1300px; margin: 0 auto 12px; color: var(--text-muted); font-size: 14px; display: flex; justify-content: space-between; }}
.results-grid {{
  max-width: 1300px; margin: 0 auto; display: grid;
  grid-template-columns: repeat(auto-fill, minmax(380px, 1fr)); gap: 16px;
}}
.card {{
  background: var(--bg-secondary); border: 1px solid var(--border); border-radius: 8px;
  padding: 16px; display: flex; flex-direction: column; justify-content: space-between;
  transition: transform 0.15s, border-color 0.15s;
}}
.card:hover {{ transform: translateY(-2px); border-color: var(--accent); }}
.card-header {{ display: flex; justify-content: space-between; align-items: flex-start; margin-bottom: 6px; }}
.card-category {{
  font-size: 11px; text-transform: uppercase; letter-spacing: 0.5px;
  color: var(--accent); font-weight: 700;
}}
.badge {{
  font-size: 10px; font-weight: 600; padding: 2px 6px; border-radius: 4px;
  background: var(--tag-lang); color: #fff; text-transform: uppercase;
}}
.card-title {{ font-size: 16px; font-weight: 600; margin-bottom: 8px; color: #fff; line-height: 1.3; }}
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
  <h1>База Знаний и Справочник Р-Про, Visual Components & Work(s) Ergo</h1>
  <p>Локальный поисковый портал по всем 1000+ темам официальной документации (CAD, Python API, Эргономика, WPP, MoCap)</p>
</div>

<div class="search-container">
  <input type="text" id="searchInput" placeholder="Поиск по API, классам, операциям (vcComponent, Snapping, L5/S1, RULA, REBA, WPP, Servo, Kinematics)..." autofocus>
</div>

<div class="filter-tabs" id="filterTabs">
  <button class="tab-btn active" data-cat="all">Все разделы ({len(portal_articles)})</button>
  <button class="tab-btn" data-cat="Python API Reference">Python API (222)</button>
  <button class="tab-btn" data-cat="R-Pro CAD Core Manual">Р-Про CAD RU (387)</button>
  <button class="tab-btn" data-cat="R-Pro / VC CAD Core Manual">CAD EN (363)</button>
  <button class="tab-btn" data-cat="Ergonomics Module">Эргономика Р-Про (14)</button>
  <button class="tab-btn" data-cat="Working Postures Module">Рабочие позы WPP (8)</button>
  <button class="tab-btn" data-cat="Motion Capture Module">Захват движения MoCap (8)</button>
  <button class="tab-btn" data-cat="Work(s) Ergo">Work(s) Ergo Guide</button>
</div>

<div class="stats" id="stats">
  <span id="matchCount">Загрузка индекса...</span>
  <span>Офлайн-индекс готов</span>
</div>
<div class="results-grid" id="resultsGrid"></div>

<script>
const articles = {json.dumps(portal_articles, ensure_ascii=False)};
let currentCat = "all";

const searchInput = document.getElementById("searchInput");
const resultsGrid = document.getElementById("resultsGrid");
const matchCount = document.getElementById("matchCount");
const filterTabs = document.getElementById("filterTabs");

function render() {{
  const query = searchInput.value.toLowerCase().trim();
  const filtered = articles.filter(a => {{
    const matchCat = (currentCat === "all") || a.category.toLowerCase().includes(currentCat.toLowerCase());
    if (!matchCat) return false;
    if (!query) return true;
    return a.title.toLowerCase().includes(query) || a.snippet.toLowerCase().includes(query);
  }});

  matchCount.textContent = `Найдено документов: ${{filtered.length}} из ${{articles.length}}`;
  resultsGrid.innerHTML = filtered.slice(0, 150).map(a => `
    <div class="card">
      <div>
        <div class="card-header">
          <div class="card-category">${{a.category}}</div>
          <span class="badge">${{a.lang}}</span>
        </div>
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

print(f"Updated portal generated at {portal_path} with {len(portal_articles)} articles.")
