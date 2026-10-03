"""
MOCAP Extractor & Analyzer for Axis Studio (Noitom Perception Neuron) BVH Files.
Part of Works Ergo R-Pro Edition.
"""

import os
import sys
import glob
import math

if hasattr(sys.stdout, 'reconfigure'):
    sys.stdout.reconfigure(encoding='utf-8')

class BVHNode:
    def __init__(self, name, parent=None):
        self.name = name
        self.parent = parent
        self.children = []
        self.offset = [0.0, 0.0, 0.0]
        self.channels = []
        self.channel_indices = []

    def __repr__(self):
        return f"BVHNode({self.name}, channels={len(self.channels)})"

class BVHParser:
    def __init__(self, filepath):
        self.filepath = filepath
        self.root = None
        self.nodes = []
        self.total_channels = 0
        self.frame_count = 0
        self.frame_time = 0.0
        self.frames = []
        self._parse()

    def _parse(self):
        with open(self.filepath, 'r', encoding='utf-8', errors='ignore') as f:
            lines = f.readlines()

        idx = 0
        stack = []
        current_node = None
        channel_cursor = 0

        while idx < len(lines):
            line = lines[idx].strip()
            if not line:
                idx += 1
                continue

            tokens = line.split()

            if tokens[0] in ('ROOT', 'JOINT'):
                name = tokens[1]
                node = BVHNode(name, stack[-1] if stack else None)
                if stack:
                    stack[-1].children.append(node)
                else:
                    self.root = node
                self.nodes.append(node)
                stack.append(node)
                current_node = node

            elif tokens[0] == 'End' and tokens[1] == 'Site':
                name = f"{current_node.name}_End" if current_node else "End_Site"
                node = BVHNode(name, current_node)
                if current_node:
                    current_node.children.append(node)
                stack.append(node)
                current_node = node

            elif tokens[0] == 'OFFSET':
                if current_node:
                    current_node.offset = [float(tokens[1]), float(tokens[2]), float(tokens[3])]

            elif tokens[0] == 'CHANNELS':
                num_ch = int(tokens[1])
                ch_names = tokens[2:2 + num_ch]
                if current_node:
                    current_node.channels = ch_names
                    current_node.channel_indices = list(range(channel_cursor, channel_cursor + num_ch))
                    channel_cursor += num_ch

            elif tokens[0] == '}':
                if stack:
                    stack.pop()
                    current_node = stack[-1] if stack else None

            elif tokens[0] == 'MOTION':
                idx += 1
                break

            idx += 1

        self.total_channels = channel_cursor

        # Parse Motion Data
        while idx < len(lines):
            line = lines[idx].strip()
            if not line:
                idx += 1
                continue

            tokens = line.split()
            if tokens[0] == 'Frames:':
                self.frame_count = int(tokens[1])
            elif tokens[0] == 'Frame' and tokens[1] == 'Time:':
                self.frame_time = float(tokens[2])
                idx += 1
                break
            idx += 1

        while idx < len(lines):
            line = lines[idx].strip()
            if line:
                values = [float(v) for v in line.split()]
                if len(values) >= self.total_channels:
                    self.frames.append(values[:self.total_channels])
            idx += 1

    def get_summary(self):
        fps = round(1.0 / self.frame_time, 1) if self.frame_time > 0 else 0
        duration_sec = round(len(self.frames) * self.frame_time, 2)
        node_names = [n.name for n in self.nodes]
        return {
            "file": os.path.basename(self.filepath),
            "size_bytes": os.path.getsize(self.filepath),
            "total_nodes": len(self.nodes),
            "total_channels": self.total_channels,
            "frame_count": len(self.frames),
            "fps": fps,
            "duration_sec": duration_sec,
            "joints": node_names
        }

def analyze_mocap_folder(folder_path):
    print("=" * 65)
    print(" AXIS STUDIO MOCAP TELEMETRY EXTRACTOR (WORKS ERGO R-PRO)")
    print("=" * 65)
    print(f"Target Directory: {folder_path}\n")

    bvh_files = glob.glob(os.path.join(folder_path, "*.bvh"))
    if not bvh_files:
        print("No .bvh files found in target directory.")
        return

    print(f"Found {len(bvh_files)} BVH recording takes.\n")

    total_all_frames = 0
    total_all_seconds = 0.0

    for idx, fpath in enumerate(bvh_files, 1):
        try:
            parser = BVHParser(fpath)
            s = parser.get_summary()
            total_all_frames += s["frame_count"]
            total_all_seconds += s["duration_sec"]

            print(f"[{idx}/{len(bvh_files)}] {s['file']}")
            print(f"     Duration: {s['duration_sec']}s ({s['frame_count']} frames @ {s['fps']} Hz)")
            print(f"     Skeletal Joints: {s['total_nodes']} nodes, {s['total_channels']} kinematic channels")
            print(f"     Key Hierarchy: {', '.join(s['joints'][:7])} ...")
            print()
        except Exception as e:
            print(f"[{idx}/{len(bvh_files)}] Error parsing {os.path.basename(fpath)}: {e}")

    print("-" * 65)
    print(f"TOTAL MOCAP DATASET AVAILABLE:")
    print(f"  Frames: {total_all_frames:,} frames")
    print(f"  Total Duration: {total_all_seconds / 60.0:.2f} minutes ({total_all_seconds:.1f} s)")
    print("=" * 65)

if __name__ == "__main__":
    target_dir = r"D:\Задания, Уроки\Запись с датчиков"
    if len(sys.argv) > 1:
        target_dir = sys.argv[1]
    analyze_mocap_folder(target_dir)
