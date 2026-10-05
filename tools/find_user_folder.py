# -*- coding: utf-8 -*-
import os
import sys

docs_dir = r"C:\Users\Jojo\Documents"
target_dir = None
for item in os.listdir(docs_dir):
    if "work(s) ergo" in item.lower():
        target_dir = os.path.join(docs_dir, item)
        break

print(f"Target folder: {target_dir}")
if target_dir:
    for f in os.listdir(target_dir):
        fp = os.path.join(target_dir, f)
        print(f"  {f} ({os.path.getsize(fp)} bytes)")
