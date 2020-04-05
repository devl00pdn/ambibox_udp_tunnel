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

This code has written after two days of C# experiance, so I hope the user will be tolerant about code quality. Thx to guides and stackoverflow :)  

## Solution description

Applications have been using in the solution:

1. AmbiBox - backlight data source. http://www.ambibox.ru/en/index.php/Main_Page
2. com2com - virtual com ports emlator. https://sourceforge.net/projects/com0com/
3. Ambibox udp tunnel - project of this repository.

com2com application creates two virtual comm ports and bridge between them.
After installation Ambibox udp tunnel starts the service.
Ambibox and Ambibox udp tunnel connect to virtual comms.
Ambibox grabs colors from monitor borders and sends to com port.
Ambibox udp tunnel service receives serial date, converts to custom packet and sends via udp to port 40000 (broadcast).

## How to use
In progress ... 

Setup process:
1) Install and config com2com
2) AmbiBox 


3) Install Ambibox udp tunnel service
  
You can get setup file from my google drive: https://drive.google.com/open?id=17uQnofNItnX8sGNvRvJ5_0ZyUmSuBFaz  
  or  build setup from sources:
  
  * Clone this repo  
  * Open project in Visual studio ( checked on VS2019) 
  * In solution explorer right click and build on ambibox_tunel, then ambibox_tunel_setup
  * In solution explorer right click ambibox_tunel_setup and select "Open directory ..", there are installation file "ambibox_tunel_setup .msi"    
  
Check that service has been successfully installed.
* Run service manager. Windows key+R, services.msc, enter.
* You should check if there is adalight_tunel_service in the list.

Service will start automatically after reboot or you can run it manual.
