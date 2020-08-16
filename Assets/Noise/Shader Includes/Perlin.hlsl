

static int p[] = { 185, 208, 195, 140, 51, 141, 231, 110, 249, 68, 73, 118, 160, 117, 250, 7, 219, 253, 170, 78, 
205, 214, 252, 8, 174, 130, 235, 169, 134, 20, 45, 109, 72, 248, 159, 190, 6, 93, 84, 240, 124, 179, 29, 64, 227, 
198, 82, 112, 35, 53, 101, 180, 153, 121, 46, 221, 207, 183, 216, 166, 151, 50, 225, 226, 19, 209, 39, 156, 203,
52, 254, 114, 157, 175, 199, 23, 177, 172, 28, 229, 43, 108, 128, 76, 161, 106, 206, 197, 193, 186, 237, 66, 122,
152, 75, 62, 202, 48, 38, 13, 145, 164, 146, 236, 49, 234, 12, 224, 155, 18, 32, 24, 139, 242, 111, 33, 162, 131,
56, 143, 0, 87, 213, 14, 69, 171, 243, 247, 91, 119, 67, 115, 125, 10, 65, 113, 211, 144, 25, 184, 94, 233, 30, 168,
142, 204, 163, 129, 40, 42, 137, 60, 251, 132, 11, 63, 212, 127, 150, 105, 244, 16, 104, 36, 200, 149, 133, 120, 167,
246, 71, 9, 97, 99, 54, 15, 58, 34, 98, 100, 2, 89, 210, 55, 228, 4, 126, 239, 245, 3, 173, 77, 232, 147, 191, 81,
92, 238, 217, 230, 61, 31, 215, 44, 201, 123, 187, 103, 83, 241, 5, 165, 27, 57, 107, 138, 74, 80, 59, 189, 26, 154,
136, 176, 86, 1, 22, 218, 182, 220, 158, 79, 85, 102, 47, 196, 116, 178, 88, 90, 41, 223, 148, 17, 222, 194, 37, 188,
21, 95, 70, 96, 192, 135, 181, 255,
185, 208, 195, 140, 51, 141, 231, 110, 249, 68, 73, 118, 160, 117, 250, 7, 219, 253, 170, 78, 205, 214, 252, 8, 174,
130, 235, 169, 134, 20, 45, 109, 72, 248, 159, 190, 6, 93, 84, 240, 124, 179, 29, 64, 227, 198, 82, 112, 35, 53, 101,
180, 153, 121, 46, 221, 207, 183, 216, 166, 151, 50, 225, 226, 19, 209, 39, 156, 203, 52, 254, 114, 157, 175, 199,
23, 177, 172, 28, 229, 43, 108, 128, 76, 161, 106, 206, 197, 193, 186, 237, 66, 122, 152, 75, 62, 202, 48, 38, 13, 145,
164, 146, 236, 49, 234, 12, 224, 155, 18, 32, 24, 139, 242, 111, 33, 162, 131, 56, 143, 0, 87, 213, 14, 69, 171, 243,
247, 91, 119, 67, 115, 125, 10, 65, 113, 211, 144, 25, 184, 94, 233, 30, 168, 142, 204, 163, 129, 40, 42, 137, 60, 251,
132, 11, 63, 212, 127, 150, 105, 244, 16, 104, 36, 200, 149, 133, 120, 167, 246, 71, 9, 97, 99, 54, 15, 58, 34, 98, 100,
2, 89, 210, 55, 228, 4, 126, 239, 245, 3, 173, 77, 232, 147, 191, 81, 92, 238, 217, 230, 61, 31, 215, 44, 201, 123, 187,
103, 83, 241, 5, 165, 27, 57, 107, 138, 74, 80, 59, 189, 26, 154, 136, 176, 86, 1, 22, 218, 182, 220, 158, 79, 85, 102,
47, 196, 116, 178, 88, 90, 41, 223, 148, 17, 222, 194, 37, 188, 21, 95, 70, 96, 192, 135, 181, 255
};

float grad(int hash, float x, float y, float z)
{
    int h = hash & 15;                                  // Take the hashed value and take the first 4 bits of it (15 == 0b1111)
    float u = h < 8 /* 0b1000 */ ? x : y;              // If the most signifigant bit (MSB) of the hash is 0 then set u = x.  Otherwise y.

    float v = 0.0;                                           // In Ken Perlin's original implementation this was another conditional operator (?:).  I
                                                       // expanded it for readability.

    if (h < 4 /* 0b0100 */)                             // If the first and second signifigant bits are 0 set v = y
        v = y;
    else if (h == 12 /* 0b1100 */ || h == 14 /* 0b1110*/)// If the first and second signifigant bits are 1 set v = x
        v = x;
    else                                                // If the first and second signifigant bits are not equal (0/1, 1/0) set v = z
        v = z;

    return ((h & 1) == 0 ? u : -u) + ((h & 2) == 0 ? v : -v); // Use the last 2 bits to decide if u and v are positive or negative.  Then return their addition.
}


float3 fade(float3 t)
{
    // Fade function as defined by Ken Perlin.  This eases coordinate values
    // so that they will "ease" towards integral values.  This ends up smoothing
    // the final output.
    return t * t * t * (t * (t * 6 - 15) + 10);         // 6t^5 - 15t^4 + 10t^3
}


float lerp(float a, float b, float x)
{
    return a + x * (b - a);
}




float Sample(float3 xyz)
{

    int xi = int(xyz.x) & 255;
    int yi = int(xyz.y) & 255;
    int zi = int(xyz.z) & 255;

    float xf = xyz.x - xi;                           // We also fade the location to smooth the result.
    float yf = xyz.y - yi;
    float zf = xyz.z - zi;

    float u = smoothstep(0.0, 1.0, xf);
    float v = smoothstep(0.0, 1.0, yf);
    float w = smoothstep(0.0, 1.0, zf);


    int a = p[xi] + yi;                             // This here is Perlin's hash function.  We take our x value (remember,
    int aa = p[a] + zi;                             // between 0 and 255) and get a random value (from our p[] array above) between
    int ab = p[a + 1] + zi;                             // 0 and 255.  We then add y to it and plug that into p[], and add z to that.
    int b = p[xi + 1] + yi;                             // Then, we get another random value by adding 1 to that and putting it into p[]
    int ba = p[b] + zi;                             // and add z to it.  We do the whole thing over again starting with x+1.  Later
    int bb = p[b + 1] + zi;                             // we plug aa, ab, ba, and bb back into p[] along with their +1's to get another set.
                                                        // in the end we have 8 values between 0 and 255 - one for each vertex on the unit cube.
                                                        // These are all interpolated together using u, v, and w below.

    float x1, x2, y1, y2;
    x1 = lerp(grad(p[aa], xf, yf, zf),          // This is where the "magic" happens.  We calculate a new set of p[] values and use that to get
        grad(p[ba], xf - 1, yf, zf),            // our final gradient values.  Then, we interpolate between those gradients with the u value to get
        u);                                     // 4 x-values.  Next, we interpolate between the 4 x-values with v to get 2 y-values.  Finally,
    x2 = lerp(grad(p[ab], xf, yf - 1, zf),          // we interpolate between the y-values to get a z-value.
        grad(p[bb], xf - 1, yf - 1, zf),
        u);                                     // When calculating the p[] values, remember that above, p[a+1] expands to p[xi]+yi+1 -- so you are
    y1 = lerp(x1, x2, v);                               // essentially adding 1 to yi.  Likewise, p[ab+1] expands to p[p[xi]+yi+1]+zi+1] -- so you are adding
                                                               // to zi.  The other 3 parameters are your possible return values (see grad()), which are actually
    x1 = lerp(grad(p[aa + 1], xf, yf, zf - 1),      // the vectors from the edges of the unit cube to the point in the unit cube itself.
        grad(p[ba + 1], xf - 1, yf, zf - 1),
        u);
    x2 = lerp(grad(p[ab + 1], xf, yf - 1, zf - 1),
        grad(p[bb + 1], xf - 1, yf - 1, zf - 1),
        u);
    y2 = lerp(x1, x2, v);

    return (lerp(y1, y2, w) + 1) * 0.5;                       // For convenience we bound it to 0 - 1 (theoretical min/max before is -1 - 1)
}



void GetPerlinValue_float(float3 pos, out float value)
{

    value = Sample(pos);

}
