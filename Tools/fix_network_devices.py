import sys
import os

def main():
    if len(sys.argv) < 2:
        print("Usage: py -3 Tools/fix_network_devices.py <path_to_file>")
        return

    file_path = sys.argv[1]

    if not os.path.isfile(file_path):
        print(f"File not found: {file_path}")
        return

    with open(file_path, "r", encoding="utf-8", errors="ignore") as f:
        lines = f.readlines()

    result = []
    block = []
    inside_block = False
    block_has_invalid = False
    invalid_blocks_count = 0  # Count of removed components

    for line in lines:
        # Start of a new component block
        if line.lstrip().startswith("- type:"):
            # Process the previous block
            if block:
                if block_has_invalid:
                    invalid_blocks_count += 1
                else:
                    result.extend(block)

            # Initialize a new block
            block = [line]
            inside_block = True
            block_has_invalid = False
            continue

        if inside_block:
            block.append(line)
            # Check for invalid entry inside the block
            if "invalid" in line.lower():
                block_has_invalid = True
        else:
            result.append(line)

    # Process the last block
    if block:
        if block_has_invalid:
            invalid_blocks_count += 1
        else:
            result.extend(block)

    with open(file_path, "w", encoding="utf-8") as f:
        f.writelines(result)

    if invalid_blocks_count > 0:
        print(f"Removed invalid components: {invalid_blocks_count}")
    else:
        print("No invalid components found.")

if __name__ == "__main__":
    main()
