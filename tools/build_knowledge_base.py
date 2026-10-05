import os
import sys
import json
import urllib.request
import ssl
import shutil
import time

def main():
    sys.stdout.reconfigure(encoding='utf-8', errors='backslashreplace')
    base_dir = r"D:\Git\WorksErgoRPro\knowledge_base"
    dirs = {
        "papers": os.path.join(base_dir, "01_scientific_papers"),
        "datasets": os.path.join(base_dir, "02_datasets"),
        "standards": os.path.join(base_dir, "03_standards_and_guidelines"),
        "rpro_internals": os.path.join(base_dir, "04_rpro_cad_internals"),
        "textbooks": os.path.join(base_dir, "05_textbooks_and_manuals"),
        "mocap": os.path.join(base_dir, "02_datasets", "mocap_axis_studio")
    }

    for d in dirs.values():
        os.makedirs(d, exist_ok=True)
    print(f"[OK] Created directories under {base_dir}")

    # Verified open-access downloads
    download_targets = [
        # Datasets & Anthropometry
        {
            "name": "ANSUR II Male Public CSV",
            "url": "https://raw.githubusercontent.com/hkair/anthropometric-stats/main/data/ansur/ANSUR%20II%20MALE%20Public.csv",
            "dest": os.path.join(dirs["datasets"], "ANSUR_II_MALE_Public.csv")
        },
        {
            "name": "ANSUR II Female Public CSV",
            "url": "https://raw.githubusercontent.com/hkair/anthropometric-stats/main/data/ansur/ANSUR%20II%20FEMALE%20Public.csv",
            "dest": os.path.join(dirs["datasets"], "ANSUR_II_FEMALE_Public.csv")
        },
        {
            "name": "ANSUR II Databases Overview PDF",
            "url": "https://raw.githubusercontent.com/hkair/anthropometric-stats/main/data/ansur/ANSUR%20II%20Databases%20Overview.pdf",
            "dest": os.path.join(dirs["datasets"], "ANSUR_II_Databases_Overview.pdf")
        },
        {
            "name": "Gordon et al. 2012 - ANSUR II Comprehensive Technical Report (Book)",
            "url": "https://raw.githubusercontent.com/hkair/anthropometric-stats/main/data/ansur/Gordon_2012_ANSURII_a611869.pdf",
            "dest": os.path.join(dirs["textbooks"], "Gordon_2012_ANSUR_II_Anthropometric_Survey.pdf")
        },
        {
            "name": "Hotzman et al. 2011 - ANSUR Anthropometric Measurement Methods",
            "url": "https://raw.githubusercontent.com/hkair/anthropometric-stats/main/data/ansur/Hotzman_2011_ANSURIII_Measurements_a548497.pdf",
            "dest": os.path.join(dirs["textbooks"], "Hotzman_2011_ANSUR_Measurement_Methods.pdf")
        },
        # OpenSim Musculoskeletal Models
        {
            "name": "OpenSim Rajagopal 2016 Full-Body Model",
            "url": "https://raw.githubusercontent.com/opensim-org/opensim-models/master/Models/Rajagopal/Rajagopal2016.osim",
            "dest": os.path.join(dirs["datasets"], "Rajagopal2016.osim")
        },
        {
            "name": "OpenSim Rajagopal-Lai-Uhlrich 2023 Updated Model",
            "url": "https://raw.githubusercontent.com/opensim-org/opensim-models/master/Models/Rajagopal/RajagopalLaiUhlrich2023.osim",
            "dest": os.path.join(dirs["datasets"], "RajagopalLaiUhlrich2023.osim")
        },
        # Scientific Papers
        {
            "name": "Pavlakos et al. 2019 - SMPL-X Expressive Body Capture (CVPR)",
            "url": "https://arxiv.org/pdf/1904.05866.pdf",
            "dest": os.path.join(dirs["papers"], "Pavlakos_2019_SMPL-X.pdf")
        },
        {
            "name": "Aristidou & Lasenby 2011 - FABRIK Fast Iterative IK Solver",
            "url": "http://www.andreasaristidou.com/publications/papers/FABRIK.pdf",
            "dest": os.path.join(dirs["papers"], "Aristidou_2011_FABRIK_IK.pdf")
        },
        {
            "name": "Buss 2004 - Introduction to Inverse Kinematics (Jacobian DLS)",
            "url": "https://www.math.ucsd.edu/~sbuss/ResearchWeb/ikmethods/iksurvey.pdf",
            "dest": os.path.join(dirs["papers"], "Buss_2004_Inverse_Kinematics_DLS.pdf")
        },
        {
            "name": "Schulman et al. 2014 - TrajOpt Sequential Convex Optimization (IJRR)",
            "url": "https://people.eecs.berkeley.edu/~pabbeel/papers/2014-IJRR-trajopt.pdf",
            "dest": os.path.join(dirs["papers"], "Schulman_2014_TrajOpt_Motion_Planning.pdf")
        },
        {
            "name": "Todorov & Jordan 2002 - Optimal Feedback Control (Nature Neuroscience)",
            "url": "https://homes.cs.washington.edu/~todorov/papers/TodorovNatNeuro02.pdf",
            "dest": os.path.join(dirs["papers"], "Todorov_2002_Optimal_Feedback_Control.pdf")
        }
    ]

    ctx = ssl._create_unverified_context()
    for item in download_targets:
        target_path = item["dest"]
        if os.path.exists(target_path) and os.path.getsize(target_path) > 1024:
            print(f"[SKIP] {item['name']} already exists ({os.path.getsize(target_path)} bytes)")
            continue

        print(f"[DOWNLOADING] {item['name']} from {item['url']} ...")
        try:
            req = urllib.request.Request(item["url"], headers={"User-Agent": "Mozilla/5.0 (Windows NT 10.0; Win64; x64)"})
            with urllib.request.urlopen(req, context=ctx, timeout=30) as resp:
                data = resp.read()
                with open(target_path, "wb") as f:
                    f.write(data)
                print(f"[SUCCESS] Saved {item['name']} ({len(data)} bytes) -> {os.path.basename(target_path)}")
        except Exception as e:
            print(f"[ERROR] Failed {item['name']}: {e}")

    # Copy / link user's local Work(s) Ergo manual and documents into knowledge base
    user_docs = r"C:\Users\Jojo\Documents\Для work(s) ergo"
    manual_pdf = os.path.join(user_docs, "Work(s) User Manual v1.17 (3).pdf")
    if os.path.exists(manual_pdf):
        dest_manual = os.path.join(dirs["textbooks"], "Works_User_Manual_v1.17.pdf")
        if not os.path.exists(dest_manual) or os.path.getsize(dest_manual) != os.path.getsize(manual_pdf):
            shutil.copy2(manual_pdf, dest_manual)
            print(f"[COPIED] Work(s) User Manual v1.17 -> {dest_manual} ({os.path.getsize(dest_manual)} bytes)")

    # Copy / link user's 12 Axis Studio BVH files into knowledge base
    mocap_source = r"D:\Задания, Уроки\Запись с датчиков"
    if os.path.exists(mocap_source):
        bvh_files = [f for f in os.listdir(mocap_source) if f.endswith(".bvh")]
        print(f"[INDEXING] Found {len(bvh_files)} Axis Studio MoCap BVH files in {mocap_source}")
        bvh_manifest = []
        for bvh in bvh_files:
            src_f = os.path.join(mocap_source, bvh)
            size = os.path.getsize(src_f)
            # Create a small summary / header file in mocap dir
            with open(src_f, "r", encoding="utf-8", errors="ignore") as f:
                header_lines = [f.readline() for _ in range(25)]
            bvh_manifest.append({
                "file": bvh,
                "size_bytes": size,
                "source_path": src_f,
                "header_sample": "".join(header_lines[:10])
            })
        manifest_file = os.path.join(dirs["mocap"], "axis_studio_bvh_manifest.json")
        with open(manifest_file, "w", encoding="utf-8") as f:
            json.dump(bvh_manifest, f, indent=2, ensure_ascii=False)
        print(f"[SUCCESS] Wrote MoCap manifest to {manifest_file} ({len(bvh_manifest)} files indexed)")

    # Create link in user's documents directory for rapid access
    link_in_docs = os.path.join(user_docs, "knowledge_base")
    try:
        if not os.path.exists(link_in_docs):
            # Try symlink or directory junction via mklink
            cmd = f'cmd /c mklink /J "{link_in_docs}" "{base_dir}"'
            os.system(cmd)
            print(f"[LINK] Created directory junction: {link_in_docs} -> {base_dir}")
    except Exception as ex:
        print(f"[NOTE] Could not create junction in docs: {ex}")

    print("\n=== Knowledge Base Archival Process Complete ===")

if __name__ == "__main__":
    main()
