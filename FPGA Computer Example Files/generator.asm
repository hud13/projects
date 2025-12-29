# generator.asm
# Hudson Dalby
#
# Count the amount of active fruits
def count_active_fruits

move_i 32 r1    # First fruit location, CHANGE TO REAL ONE
move_i 64 r2    # End of stored fruit location
move_i 0 r3     # r3 = active count 

def count_active_loop
    # Check if current fruit (r1) is at the last slot (r2)
    cmp r2 r1
    j_ge 20

    # Continue, load X,Y,V
    load_01 r1 r4    # X
    addu_i 1 r1
    load_01 r1 r5    # Y
    addu_i 1 r1
    load_01 r1 r6    # V

    # r7 = active or not
    move r4 r7
    rsh_i 15 r7

    # If r7 != 0, it's active
    cmp_i 0 r7
    j_eq 6               # Jump to find free slot if inactive
        addu_i 1 r3      # Increment active counter 

    jump count_active_loop

    # Count active loop is done.
    move r3 r0
    jumpr r15

# Finds empty fruit slot 
def find_free_slot 
    move_i 32 r1    # r1 = Fruit base location
    move_i 64 r2    # r2 = fruit end location 

def find_free_loop
    # End if r1 >= fruit end location 
    cmp r1 r2
    j_gt 4          
        # Ends loop
        move_i 0 r0     
        jumpr r15

    # Continue loop
    # Save start address of this slot in r7
    move r1 r7

    # Load X,Y,V
    load_01 r1 r4
    addu_i 1 r1
    load_01 r1 r5
    addu_i 1 r1
    load_01 r1 r6

    # Check if fruit is active r8 
    move r4 r8
    rsh_i 15 r8
    cmp_i 0 r8                   
    j_ne find_free_loop    # Keep searching if fruit is active

    # Inactive slot: Returns slotStart in r7
    move r7 r0
    jumpr r15

def generator_step
    # Push RA
    store_11 r15 r14
    add_i 1 r14

    # Load the fruit limit 
    move_i 22 r1     # CHANGE TO FRUIT LIMIT ADDRESS or IMMEDIATE CAP
    load_01 r1 r2    # r2 = fruit limit

    # Count current fruits
        jumpa
        {count_active_fruits}
    move r0 r3        # r3 = active count 

    # if active count = fruit limit, skip 
    cmp r2 r3
    j_le 40

    # find free slot 
        jumpa 
        {find_free_slot}
    move r0 r4    # r4 = free slot start

    cmp_i 0 r4
    j_eq 40       # Skip if no free slot 

    # Get random value using rng
    rng r5    

    # Compute spawn position at top of screen (0 - 511)
    move r5 r7    
    moveu_i 255 r8    # 0x00FF
    moveu_i 1   r9
    lsh_i 8 r9
    or r8 r9          # 0x01FF
    and r9 r7         # r7 = spawn X 

    # Set ACTIVE flag in X
    move_i 1 r5
    lsh_i 15 r5      
    or r5 r7

    # Spawn Y near top 
    move_i 16 r10     # r10 = spawn Y 

    # Compute velocity - Small sideways movement, constant downward
    # Vx = (random & 7) + 4 -> Slight x velocity between 11 and 4, no negatives  
    move r5 r11
    moveu_i 7 r12
    and r12 r11
    addu_i 4 r11

    # Vy = 16 (Can change)
    move_i 16 r13

    # Pack values into V
    move r13 r8
    lsh_i 8 r8
    or r11 r8    # r8 = V 

    # Write fruit into memory 
    move r4 r1        # r1 = fruit slot start

    store_01 r7 r1    # X
    addu_i 1 r1       
    store_01 r10 r1   # Y
    addu_i 1 r1
    store_01 r8 r1    # V

    # Pop RA and Return 
    # Skip spawn jumps here 
    sub_i 1 r14
    load_11 r14 r15
    jumpr r15
    