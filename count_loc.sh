#!/bin/bash

# Count lines of code in C# files
cs_count=$(find . -name '*.cs' -not -path "./obj/*" -not -path "./bin/*" -exec cat {} + | wc -l)
echo "Total lines of C# code: $cs_count"

# Count lines of code in Avalonia XAML files
axaml_count=$(find . -name '*.axaml' -not -path "./obj/*" -not -path "./bin/*" -exec cat {} + | wc -l)
echo "Total lines of Avalonia XAML code: $axaml_count"
