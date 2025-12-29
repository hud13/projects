////////////////////////////////////////////////////////////////////////////////
//
// Engineer: Hudson Dalby
//
// Dependencies: vga_glyphMemory_row.v, glyph_words_rom.v, vga_multipleGlyphBitgen.v
//		 vga_controller.v, vga_clockDivider.v
// Description:  
//   A driver module to display 5 glyphs (16x16 pixels) with arbitrary preloaded
//   pixel data and positions to a vga output. Connects a raw pixel data module
//   with a bit processing FSM 5 separate times and utilizes correct display modules
//   for our VGA config. 
//
//////////////////////////////////////////////////////////////////////////////////

module vga_fiveGlyphs_top (
   input  wire       clk,     
    input  wire       reset,      

    output wire       hsync,
    output wire       vsync,
    output wire [7:0] red_out,
    output wire [7:0] green_out,
    output wire [7:0] blue_out,
    output wire       vga_sync,
    output wire       pixelClock
);
    localparam NUM_GLYPHS = 5;
    localparam glyph_H    = 16;

    // VGA timing
    wire [9:0] hcount, vcount;
    wire       bright;
    wire       red_bit, green_bit, blue_bit;

    // Clock and controller
    vga_clockDivider clock_div (
        .clock_in (clk),
        .reset    (reset),
        .clock_out(pixelClock)
    );

    vga_controller controller (
        .clk   (pixelClock),
        .reset (reset),
        .bright(bright),
        .hsync (hsync),
        .vsync (vsync),
        .hcount(hcount),
        .vcount(vcount)
    );

    // Hard coded glyph positions
    wire [9:0] gx0 = 10'd10,  gy0 = 10'd10;
    wire [9:0] gx1 = 10'd180,  gy1 = 10'd130;
    wire [9:0] gx2 = 10'd270,  gy2 = 10'd50;
    wire [9:0] gx3 = 10'd110, gy3 = 10'd370;
    wire [9:0] gx4 = 10'd430, gy4 = 10'd290;

    wire [NUM_GLYPHS*10-1:0] xFlat = {gx4, gx3, gx2, gx1, gx0};
    wire [NUM_GLYPHS*10-1:0] yFlat = {gy4, gy3, gy2, gy1, gy0};

    // Hard coded memory 
    localparam [48*16-1:0] WORDS0 = {48{16'h0707}}; 
    localparam [48*16-1:0] WORDS1 = {48{16'h0F0F}};
    localparam [48*16-1:0] WORDS2 = {48{16'h4444}};
    localparam [48*16-1:0] WORDS3 = {48{16'hF0F0}};
    localparam [48*16-1:0] WORDS4 = {48{16'h3333}};

    // base_addr for each glyph in its local 48-word space
    localparam [9:0] BASE0 = 10'd0;
    localparam [9:0] BASE1 = 10'd0;
    localparam [9:0] BASE2 = 10'd0;
    localparam [9:0] BASE3 = 10'd0;
    localparam [9:0] BASE4 = 10'd0;

    // Signals per glyph
    wire [3:0]  rowIndex0, rowIndex1, rowIndex2, rowIndex3, rowIndex4;
    wire        rowReq0,   rowReq1,   rowReq2,   rowReq3,   rowReq4;
    wire [9:0]  bram_addr0, bram_addr1, bram_addr2, bram_addr3, bram_addr4;
    wire [15:0] bram_q0,    bram_q1,    bram_q2,    bram_q3,    bram_q4;
    wire [47:0] rowData0,   rowData1,   rowData2,   rowData3,   rowData4;
    wire        rowValid0,  rowValid1,  rowValid2,  rowValid3,  rowValid4;

    // Row index and request logic for each glyph:
    assign rowIndex0 = (vcount >= gy0 && vcount < gy0 + glyph_H) ? (vcount - gy0) : 4'd0;
    assign rowReq0   = (hcount == 10'd0) &&
                       (vcount >= gy0) && (vcount < gy0 + glyph_H);

    assign rowIndex1 = (vcount >= gy1 && vcount < gy1 + glyph_H) ? (vcount - gy1) : 4'd0;
    assign rowReq1   = (hcount == 10'd0) &&
                       (vcount >= gy1) && (vcount < gy1 + glyph_H);

    assign rowIndex2 = (vcount >= gy2 && vcount < gy2 + glyph_H) ? (vcount - gy2) : 4'd0;
    assign rowReq2   = (hcount == 10'd0) &&
                       (vcount >= gy2) && (vcount < gy2 + glyph_H);

    assign rowIndex3 = (vcount >= gy3 && vcount < gy3 + glyph_H) ? (vcount - gy3) : 4'd0;
    assign rowReq3   = (hcount == 10'd0) &&
                       (vcount >= gy3) && (vcount < gy3 + glyph_H);

    assign rowIndex4 = (vcount >= gy4 && vcount < gy4 + glyph_H) ? (vcount - gy4) : 4'd0;
    assign rowReq4   = (hcount == 10'd0) &&
                       (vcount >= gy4) && (vcount < gy4 + glyph_H);

    // 5 word ROMs (Separate memories but functionally the same)
    glyph_words_rom #(.WORDS(WORDS0)) rom0 (
        .clk (pixelClock),
        .addr(bram_addr0),
        .q   (bram_q0)
    );

    glyph_words_rom #(.WORDS(WORDS1)) rom1 (
        .clk (pixelClock),
        .addr(bram_addr1),
        .q   (bram_q1)
    );

    glyph_words_rom #(.WORDS(WORDS2)) rom2 (
        .clk (pixelClock),
        .addr(bram_addr2),
        .q   (bram_q2)
    );

    glyph_words_rom #(.WORDS(WORDS3)) rom3 (
        .clk (pixelClock),
        .addr(bram_addr3),
        .q   (bram_q3)
    );

    glyph_words_rom #(.WORDS(WORDS4)) rom4 (
        .clk (pixelClock),
        .addr(bram_addr4),
        .q   (bram_q4)
    );

    // 5 glyphMemory_row FSMs
    vga_glyphMemory_row gm0 (
        .clk       (pixelClock),
        .reset     (reset),
        .rowRequest(rowReq0),
        .rowIndex  (rowIndex0),
        .base_addr (BASE0),
        .bram_q    (bram_q0),
        .bram_addr (bram_addr0),
        .rowData   (rowData0),
        .rowValid  (rowValid0)
    );

    vga_glyphMemory_row gm1 (
        .clk       (pixelClock),
        .reset     (reset),
        .rowRequest(rowReq1),
        .rowIndex  (rowIndex1),
        .base_addr (BASE1),
        .bram_q    (bram_q1),
        .bram_addr (bram_addr1),
        .rowData   (rowData1),
        .rowValid  (rowValid1)
    );

    vga_glyphMemory_row gm2 (
        .clk       (pixelClock),
        .reset     (reset),
        .rowRequest(rowReq2),
        .rowIndex  (rowIndex2),
        .base_addr (BASE2),
        .bram_q    (bram_q2),
        .bram_addr (bram_addr2),
        .rowData   (rowData2),
        .rowValid  (rowValid2)
    );

    vga_glyphMemory_row gm3 (
        .clk       (pixelClock),
        .reset     (reset),
        .rowRequest(rowReq3),
        .rowIndex  (rowIndex3),
        .base_addr (BASE3),
        .bram_q    (bram_q3),
        .bram_addr (bram_addr3),
        .rowData   (rowData3),
        .rowValid  (rowValid3)
    );

    vga_glyphMemory_row gm4 (
        .clk       (pixelClock),
        .reset     (reset),
        .rowRequest(rowReq4),
        .rowIndex  (rowIndex4),
        .base_addr (BASE4),
        .bram_q    (bram_q4),
        .bram_addr (bram_addr4),
        .rowData   (rowData4),
        .rowValid  (rowValid4)
    );

    // Hold latest valid row for each glyph
    reg [47:0] curRow0, curRow1, curRow2, curRow3, curRow4;

    always @(posedge pixelClock) begin
        if (!reset) begin
            curRow0 <= 48'd0;
            curRow1 <= 48'd0;
            curRow2 <= 48'd0;
            curRow3 <= 48'd0;
            curRow4 <= 48'd0;
        end else begin
            if (rowValid0) curRow0 <= rowData0;
            if (rowValid1) curRow1 <= rowData1;
            if (rowValid2) curRow2 <= rowData2;
            if (rowValid3) curRow3 <= rowData3;
            if (rowValid4) curRow4 <= rowData4;
        end
    end

    // Flatten for the bitgen
    wire [NUM_GLYPHS*48-1:0] dataInFlat = {curRow4, curRow3, curRow2, curRow1, curRow0};

    // Multi glyph bitgen 
    vga_multipleGlyphBitgen #(.NUM_GLYPHS(NUM_GLYPHS)) bitgen (
        .pixelClock(pixelClock),
        .bright    (bright),
        .hcount    (hcount),
        .vcount    (vcount),
        .xFlat     (xFlat),
        .yFlat     (yFlat),
        .dataInFlat(dataInFlat),
        .red       (red_bit),
        .green     (green_bit),
        .blue      (blue_bit)
    );

    // Expand to 8-bit VGA channels
    assign red_out   = {8{red_bit}};
    assign green_out = {8{green_bit}};
    assign blue_out  = {8{blue_bit}};
    assign vga_sync  = 1'b0;

endmodule