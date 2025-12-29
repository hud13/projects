# Collision.asm
# Hudson Dalby
#
# check for collisions and update the fruit cut state if a collision occurs
def check_collision
    # Pull blade data
    # Loop over fruit
        # Pull fruit data
        # Check for collision and update cut state
        # save new fruit state
    # return
    # Load blade position 
    moveu_i 16 r1
    load_01 r2 r1    # r2 = x of blade
    add_i 1 r1
    load_01 r3 r1    # r3 = y of blade

    # Fruit data table 
    moveu_i 1 r4    
    lsh_i 8 r4       # Replace wherever fruit data address starts

    moveu_i 5 r5     # Max fruit slots = 5 (can change or get game variable address)
    moveu_i 0 r6     # i = 0

# Loop over all fruit for collision 
def coll_loop
    cmp r5 r6
    j_eq coll_done   # Done with all fruit slots

    # update current fruit address
    # current address = base + i*3
    move r4 r7
    move r6 r8
    move r0 r8
    lsh_i 1 r8    # i*2
    add r0 r8     # i*2 + i 
    add r8 r7     # r7 = current address

    load_01 r9 r7    # Load current fruit state 
    rsh_i 15 r9      # Get the current active state of the fruit 
    cmpu_i 1 r9
    j_ne 30          # Skips current fruit if inactive

        # Load fruit x,y 
        add_i 1 r7
        load_01 r10 r7    # Fruit X
        add_i 1 r7 
        load_01 r11 r7    # Fruit Y 

        # Check for collision with simple box check
        # if BladeX + 16 > FruitX and fruitX + 16 > bladeX, collision occurs. Same with Y

        # Check FruitX < BladeX + 16
        move r2 r12
        add_i 16 r12
        cmp r10 r12
        j_ge 34

        # Check BladeX < FruitX + 16
        move r10 r13
        add_i 16 r13
        cmp r2 r13
        j_ge 28

        # Check Fruit Y < BladeY + 16
        move r3 r12
        add_i 16 r12
        cmp r11 r12
        j_ge 22

        # Check BladeY < FruitY + 16
        move r11 r13
        add_i 16 r13
        cmp r3 r13
        j_ge 16

            # Collision occurs - Set fruit to inactive and increment score 
            add_i 1024 r11    # Change fruit collision variable to 1 in Y 
            store_01 r11 r7   # Store updated fruit Y

            sub_i 1 r7        # Revert address back to X fruit data 
            add_i 1024 r10    # Change fruit cut variable in X
            store_01 r10 r7   # Store updated fruit X

            # Increment Score 
            moveu_i 3333 r0   # CHANGE TO SCORE ADDRESS
            load_01 r1 r0
            add_i 1 r1
            store_01 r1 r0

    # Increment i, move to next fruit 
    add_i 1 r6
    jump coll_loop 

def coll_done
    jumpr r15