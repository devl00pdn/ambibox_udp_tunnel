# Ambibox udp tunnel
### Platform: Windows 10 | type: service  

This repo contains the solution for translating serial data form Ambibox (only Adalight device config) to custom packet, that then sends broadcast via udp.  

#### Hardcoded configs: serial port "COM5" | udp broadcast port 40000
#### Custom packet structure:
```c++
  struct leds_data{
    uint16_t magic; /// always 0xADAF
    int16_t leds_count; /// equal "Number of zones" config in Adalight app
    /// every led coded by three continuous bytes that contain amplitudes: red, green, blue
    uint8_t led_rgb_state[leds_count*3]; 
  }
```

This code writed after two day C# experiance, so I hope the user will be tolerant about code quality. Thx to guides and stackoverflow :)  

## Solution description: in progress ...
 
