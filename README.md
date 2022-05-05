# WaveGenerator
Uses Gerstner Waves and a layered noise map (Fractal Brownian Motion I think) to create wave shapes.

![Wave Gif](https://media.giphy.com/media/lWS8ySFPFM3acEyFKE/giphy.gif)

The whitecaps were created by taking the Y component of each surface normal, multiplying it by the normalized height of each associated vertex, then evaluating that value along a gradient.
