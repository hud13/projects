# fruit.asm
# Hudson Dalby

# (r1=fruitStartAddress) -> fruitStartAddress
# update a single fruit 
def update_fruit_single

    # pull fruit data
    load_01 r1 r2       # r2 = fruit x
    addu_i 1 r1
    load_01 r1 r4       # r4 = fruit y
    addu_i 1 r1
    load_01 r1 r6       # r6 = fruit v

    # check for spawn
    move_i 1 r3
    lsh_i 15 r3         # r3 = 1000 0000 0000 0000
    test r3 r2
    j_eq 2              # if (r2 && r3 == 0) return
        jumpr r15

    # extract data
    move r2 r3
    move r4 r5
    move r6 r8
    move_i -1 r10
    rsh_i 6 r10          # r10 = 0000 0011 1111 1111
    and r10 r2           # r2 = fruit x-position
    and r10 r4           # r4 = fruit y-position
    not r10 r10 
    and r10 r3           # r3 = fruit state 1
    and r10 r5           # r5 = fruit state 2
    moveu_i 255 r10      # r10 = 0000 0000 1111 1111
    and r10 r6           # r6 = fruit x-velocity decimal
    move r6 r7
    rsh_i 2 r7           # r7 = fruit x-velocity integer
    rsh_i 8 r8           # r8 = fruit y-velocity decimal
    move r8 r9
    rsh_i 2 r9           # r9 = fruit y-velocity integer

    # update position
    addu r7 r2
    addu r9 r4

    cmp_i 16 r2
    j_lt 29
    j_lt 47
    cmp_i 16 r4
    j_lt 27
    j_lt 45
    moveu_i 39 r10
    lsh_i 4 r10
    cmp r10 r2
    j_gt 23
    j_gt 41
    moveu_i 29 r10
    lsh_i 4 r10
    cmp r10 r4
    j_gt 19
    j_gt 37

    # update velocity
    addu_i 1 r8
@@ -70,32 +70,50 @@
    move r1 r0
    jumpr r15

    # Fruit is out of bounds 
    # Check if fruit has been cut (BIT-10)
    moveu_i 1024 r11
    move r3 r12
    and r11 r12
    j_ne 12

    move r5 r12
    and r11 r12
    cmp_i 0 r12
    j_ne 7

    # If fruit is not cut, mistakes++
    move_i 24 r13    # Mistakes address
    load_01 r14 r13
    add_i 1 r14
    store_01 r14 r13

    # Save unspawned result
    move_i 0 r2
    store_01 r2 r1
    sub_i 1 r1
    store_01 r2 r1
    sub_i 1 r1
    store_01 r2 r1
    move r1 r0
    jumpr r15

# () -> None
# update all position info for fruits
def update_fruit
    store_11 r15 r14
    add_i 1 r14

    # Loop over all fruit and update them
    move_i 32 r1    # r0 = 0000 0000 0010 0000
        jumpa
        {update_fruit_single}
        move r0 r1
        add_i 1 r1
        move_i 64 r2    # r1 = 0000 0000 0100 0000
        cmp r2 r1
    j_b -6

    sub_i 1 r14
    load_11 r14 r15
    jumpr r15