////////////////////////////////////////////////////////////////////////////////
//
// Engineer: Hudson Dalby
//
// Tests:        the_board\tests\components\nes_controller\nes_controller_tb.v
// Dependencies: None
// Description:  
//   A core using an FSM to read and update button presses by the player. 
//   Uses an 8-bit bitmask to represent controller state. 
//   Same bit ordering as original NES system. 
//   
//   Bit mapping (active high):
//   [0] - A
//   [1] - B
//   [2] - Select
//   [3] - Start
//   [4] - Up
//   [5] - Down
//   [6] - Left
//   [7] - Right
//
//	  50 MHz board clock rate used
//   Sampling rate defaulted to 1000 HZ (can be adjusted)
//   Latch and clk periods may also be adjusted if needed. 
//
//////////////////////////////////////////////////////////////////////////////////

module nes_controller_driver #(
    parameter integer BOARD = 50_000_000,
    parameter integer SAMPLE= 1000,
    parameter integer LATCH = 20, 
    parameter integer CLK   = 5    
)(
    input  wire clk,
    input  wire reset,

    output reg  nes_latch,
    output reg  nes_clk,
    input  wire nes_data,

    output reg [7:0] buttons,
    output reg       ready
);

    // Tick counts
    localparam integer sample_ticks = BOARD / SAMPLE;
    localparam integer latch_ticks  = (BOARD / 1_000_000) * LATCH;
    localparam integer half_clk     = (BOARD / 1_000_000) * (CLK / 2);

    // NES data is active-low
    wire data_bit = ~nes_data;

    // FSM states
    localparam [2:0]
        S0 = 3'd0,
        S1 = 3'd1,
        S2 = 3'd2,
        S3 = 3'd3,
        S4 = 3'd4,
        S5 = 3'd5,
        S6 = 3'd6;

    reg [2:0]  state;
    reg [31:0] count;
    reg [2:0]  bit_idx; 
    reg [7:0]  shift;

    always @(posedge clk or posedge reset) begin
        if (reset) begin
            state     <= S0;
            count     <= 32'd0;
            bit_idx   <= 3'd0;
            shift     <= 8'd0;
            buttons   <= 8'd0;
            nes_latch <= 1'b0;
            nes_clk   <= 1'b0;
            ready     <= 1'b0;
        end else begin
            ready <= 1'b0; 

            case (state)

                // Wait between polls
                S0: begin
                    nes_latch <= 1'b0;
                    nes_clk   <= 1'b0;

                    if (count >= sample_ticks - 1) begin
                        count     <= 32'd0;
                        nes_latch <= 1'b1; // start latch pulse
                        state     <= S1;
                    end else begin
                        count <= count + 1;
                    end
                end

                // Latch high: controller captures buttons
                S1: begin
                    nes_latch <= 1'b1;
                    nes_clk   <= 1'b0;

                    if (count >= latch_ticks - 1) begin
                        count     <= 32'd0;
                        nes_latch <= 1'b0; // end latch
                        nes_clk   <= 1'b0;
                        bit_idx   <= 3'd0; // start at bit 0 (A)
                        state     <= S2;
                    end else begin
                        count <= count + 1;
                    end
                end

                // Wait for stabilization after latch low
                S2: begin
                    nes_clk <= 1'b0;
                    if (count >= half_clk - 1) begin
                        count <= 32'd0;
                        state <= S3;  
                    end else begin
                        count <= count + 1;
                    end
                end

					 // Index check, Check if more indexes need to be sampled or move to done
                S3: begin
                    nes_clk          <= 1'b0;
                    shift[bit_idx]   <= data_bit;

						  // Check if on last index
                    if (bit_idx == 3'd7) begin
                        state <= S6;
                    end else begin
                        // Move to next state
                        count <= 32'd0;
                        state <= S4;
                    end
                end

                // Clock high, rising edge shifts the values
                S4: begin
                    nes_clk <= 1'b1;
                    if (count >= half_clk - 1) begin
                        count <= 32'd0;
                        state <= S5;
                    end else begin
                        count <= count + 1;
                    end
                end

                // Clock low: complete the pulse, then go sample next bit
                S5: begin
                    nes_clk <= 1'b0;
                    if (count >= half_clk - 1) begin
                        count   <= 32'd0;
                        bit_idx <= bit_idx + 1; // move to next bit
                        state   <= S3;
                    end else begin
                        count <= count + 1;
                    end
                end

                // Copy shift to buttons, assert ready, go back to Idle
                S6: begin
                    nes_latch <= 1'b0;
                    nes_clk   <= 1'b0;
                    buttons   <= shift;
                    ready     <= 1'b1;
                    count     <= 32'd0;
                    state     <= S0;
                end

					 // Default state is idle
                default: state <= S0;

            endcase
        end
    end

endmodule
					
