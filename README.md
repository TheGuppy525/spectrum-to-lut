# Spectrum-to-lut
Tool to create 3D LUT files from a modified hue spectrum.
Spectrum-to-lut is a command line program written in C#.

# About
This tool was made for creating LUTs for use with [Resonite](https://resonite.com/) but should work with other applications aswell.
A good tool for creating hue spectrums was created for [Color in Color](https://www.color-in-color.info/color-in-color/1d2d3d-texture-designer).

# Usage
### 1. Specify input spectrum.
`spectrum-to-lut -i [PATH]`
Outputs a '.cube' file by default.

### 2. You can optionally specifiy and output file.
`spectrum-to-lut -o [PATH]`
Note that the file extension will be added automatically to the output path.

### 3. You can also specifiy the output format.
`spectrum-to-lut -f [FORMAT]`
Most common image formats are supported.

### 4. The size of the output LUT can be changed (default is 32).
`spectrum-to-lut -s [SIZE]`

### 5. A offset in pixels from the top of the input spectrum can be specified\n
if you have multiple spectrums in your input file.
`spectrum-to-lut -offset [PIXELS]`
