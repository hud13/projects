# game.asm
# Hudson Dalby

# Initialize game variables 
def init_game 
    # Set fruit limit
    move_i 22 r1    # Fruit limit addr.
    move_i 5  r2    # Fruit limit is defaulted to 5
    store_01 r2 r1    

    # Set score = 0
    move_i 23 r1    # Score addr.
    move_i 0 r2 
    store_01 r2 r1

    # Set Mistakes = 0
    move_i 24 r1    # Mistakes addr.
    move_i 0 r2 
    store_01 r2 r1

    jumpr r15

# Wait for frame buffer
def wait_for_frame
    move_i 25 r1    # Frame counter addr.
    load_01 r1 r2   # r2 = old frame
    
    load_01 r1 r3   # r3 = new frame 
    cmp r3 r2 
    j_eq -3         # Keep looping until frame changes
    jumpr r15

 # Able to add small start game menu or loop here
def start_game 

    jumpa 
    {init_game}

# Main game loop 
#    1) Update glyphs
#    2) Update fruit positions and cleanup fruit
#    3) Update blade position
#    4) Check for collisions 
#    5) Generate random fruit 
#    6) Wait for frame buffer
def game_loop 

    jumpa
    {update_glyphs}

    jumpa
    {update_fruit}

    jumpa
    {update_blade}

    jumpa
    {check_collision}

    jumpa 
    {generator_step}

    jumpa
    {wait_for_frame}

jumpa
game_loop