# glyphs.asm
# Hudson Dalby

# go through all of the objects in the game and translate them to glyphs in memory
def update_glyphs
    store_11 r15 r14
    add_i 1 r14

    moveu_i 33 r1
    lsh_i 4 r1              # r1 = 0000 0010 0001 0000

    # First write blade glyph
    jumpa
    {update_glyphs_blade}
    move r0 r1

    # Write fruit glyphs
    jumpa
    {update_glyphs_fruit}

# (pointerToGlyphMemory) -> newPointerToGlyphMemory
# save the glyphs for the blade
def update_glyphs_blade
    # pull blade data
    # convert to glyph data
    # save glyph data
    # return new pointer

    # Save glyph memory pointer 
    move r1 r10        # r10 = glyphPointer

    # Load blade X and Y 
    moveu_i 16 r2        # BladeXAddress
    load_01 r2 r3        # r3 = bladeX
    add_i 1 r2
    load_01 r2 r4        # r4 = bladeY

    # Write glyph data to glyph memory 
    store_10 r3 r10      # [0] = bladeX
    add_i 1 r10
    store_10 r4 r10      # [1] = bladeY
    add_i 1 r10

    # Return new glyph pointer
    move r10 r0 
    jumpr r15
    
# (pointerToGlyphMemory) -> newPointerToGlyphMemory
# save the glyphs for the fruit
def update_glyphs_fruit
    # loop over fruit
        # pull fruit data
        # convert to glyph data
        # save glyph data
    # return new pointer

    # r1 = passed glyph memory pointer
    move r1 r10     

    # Set up fruit boundaries 
    move_i 32 r1    # Fruit starting location 
    move_i 64 r2    # Fruit ending location 

def update_glyphs_fruit_loop
    # Check if we've looped through all fruits
    cmp r2 r1
    j_le 40

    move r1 r7    # r7 = Fruit slot X

    # Load X, Y, V from 01 memory space
    load_01 r1 r3    # r3 = X
    add_i 1 r1
    load_01 r1 r4    # r4 = Y
    add_i 1 r1
    load_01 r1 r5    # r5 = V
    add_i 1 r1       # R1 points to next fruit

    # Check if fruit is active
    move r3 r6
    rsh_i 15 r6
    cmp_i 0 r6
    j_eq 22

    # Active fruit: Extract X and Y position (Color and glyph data too, add later)
    move r3 r6        # r6 = X
    move_i 1023 r8    # r8 = 0000 0011 1111 1111
    and r8 r6         # r6 = xPosition 

    move r4 r9        # r9 = Y
    move_i 1023 r8    # r8 = 0000 0011 1111 1111
    and r8 r9         # r9 = yPositon 

    # Pack RGB, Glyph type data here 

    # Write Glyph to memory 
    store_10 r6 r10    # [0] = xPosition 
    add_i 1 r10        
    store_10 r9 10     # [1] = yPosition 
    add_i 2 r10        # next pointer 

    jumpa update_glyphs_fruit_loop

    # Fruit glyph loop is done, return pointer
    move r10 r0 
    jumpr r15