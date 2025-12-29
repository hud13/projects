////////////////////////////////////////////////////////////////////////////////
//
// Engineer: Hudson Dalby
//
// Tests:         None
// Dependencies:  ram_dualPort1024.v, sevSeg_16b.v
// Description: 
//   One of Two FSM modules for testing of dual-port RAM.
//	  Tests Read/Write of A and B ports operating on the same memory location
//		Board test pinouts are at bottom of file.  
//
// Additional Comments:
//    Our components are not setup to handle simultaneous write operations
//		to the same memory location. Be wary of this for both read and write operations
//		in the future unless addressed.
//
////////////////////////////////////////////////////////////////////////////////

module fsm_dualPortB #(

	// Parameter variables (DataWidth and AddressWidth + hold variable for testing purposes)
	parameter dw = 16,
	parameter aw = 10,
	parameter hold = 3,
	parameter ACTIVE_LOW = 1
	
)
(
	// Setup inputs to RAM module
	input wire clk_a,
	input wire clk_b,
	input wire reset,
	input wire enable,
	input wire select,
	
	// 7-seg display output
	output wire [6:0]	seg3,
	output wire [6:0]	seg2,
	output wire [6:0]	seg1,
	output wire [6:0]	seg0
	);
	 
	 
	// Setup wire outputs to work with testbench
	wire [dw-1:0] q_a;
	wire [dw-1:0] q_b;
	
	// Setup reg outputs to work with testbench
	reg [3:0] 	  state;
	reg			  we_a;
	reg			  we_b;
	reg [dw-1:0] data_a;
	reg [dw-1:0] data_b;
	reg [aw-1:0] addr_a;
	reg [aw-1:0] addr_b;
	
	
	// Instantiate RAM module 
	ram_dualPort1024 #(.DATA_WIDTH(dw), .ADDR_WIDTH(aw)) uut (
		.data_a(data_a), .addr_a(addr_a), .we_a(we_a), .clk_a(clk_a), .q_a(q_a),
		.data_b(data_b), .addr_b(addr_b), .we_b(we_b), .clk_b(clk_b), .q_b(q_b)
	);
	
	// Hold time so A and B complete full write/read operation
	integer count;
	
	// Done 
	integer done;
	parameter last_state = 13;
	
	// FSM uses clock A to advance
    always @(posedge clk_a) begin
       if (!reset) begin
			state <= 4'd0;
			count <= 0;
			done <= 0;
       end else if (enable && !done) begin 
			if (count != 0) begin
				count <= count - 1'b1;
			end else begin
			
			// Advance and reset the delay count 
			if (state < last_state) begin
			state <= state + 1'b1;
			count <= (hold > 0) ? (hold - 1) : 0;
			
			// Flag done when last state is reached
			if ((state + 1) == last_state)
				done <= 1;
				
			// Remain in last state
			end else begin 
			state <= last_state;
			count <= 0;
			done <= 1;
			end
		 end
    end
	end
	 
	 // Begins state machine
	 always @(state) begin
		 
		 // Defaults for every state
		 we_a = 1'b0;
		 we_b = 1'b0;
		 
		 data_a = 16'b0;
		 data_b = 16'b0;
		 addr_a = 10'b0;
		 addr_b = 10'b0;
		 
		 //Begin state machine
		 case (state)
		 
			// A writes 0 to first address for initial state
			0: begin 
				addr_a = 0; data_a = 16'h0000; we_a = 1;
			end
		 
			// A writes to arbitrary location
			1:	begin 
				addr_a = 0; data_a = 16'h000A; we_a = 1; 
			end
		 
			// B writes to arbitrary location
			2: begin 
				addr_b = 5; data_b = 16'h000B; we_b = 1;
			end
			
			// A reads B's written location 
			3: begin 
				addr_a = 5;
			end
			
			// B reads A's written location 
			4: begin
				addr_b = 0;
			end
			
			// A writes new value to B's location
			5: begin
				addr_a = 5; data_a = 16'h007A; we_a = 1;
			end
				
			// B writes new value to A's location 
			6: begin
				addr_b = 0; data_b = 16'h003B; we_b = 1;
			end 
				
			// A reads its previously written location
			7: begin 
				addr_b = 0;
			end
		 
			// B reads its previously written location 
			8: begin 
				addr_a = 5;
			end
				
			// A and B both write to new locations 
			9: begin 
				addr_a = 3; data_a = 16'h1234; we_a = 1;
				addr_b = 8; data_b = 16'h5678; we_b = 1;
			end 
			
			// A and B read the other's location
			10: begin 
				addr_a = 8;
				addr_b = 3;
			end
			
			// A and B write to the other's location 
			11: begin
				 addr_a = 8; data_a = 16'h90AB; we_a = 1;
				 addr_b = 3; data_b = 16'hCDEF; we_b = 1;
			end
				 
			// A and B read previously written locations 
			12: begin 
				 addr_a = 3;
				 addr_b = 8; 
			end
				 
			// A and B read newly written locations 
			13: begin 
				addr_a = 8;
				addr_b = 3;
			end
			
			// Do nothing as default
			default: begin 
			end
			
		endcase
	end 
	
	// Seven segment display 
	
	wire [dw-1:0] displayVal = select ? q_b : q_a;	
	
	sevSeg_16b display(
	.value(displayVal),
	.seg3(seg3),
	.seg2(seg2),
	.seg1(seg1),
	.seg0(seg0)
	);
	
	
endmodule


//set_location_assignment PIN_AA16 -to clk_a
//set_location_assignment PIN_Y26 -to clk_b
//set_location_assignment PIN_AB12 -to select
//set_location_assignment PIN_AA14 -to reset
//set_location_assignment PIN_AH28 -to seg0[6]
//set_location_assignment PIN_AG28 -to seg0[5]
//set_location_assignment PIN_AF28 -to seg0[4]
//set_location_assignment PIN_AG27 -to seg0[3]
//set_location_assignment PIN_AE28 -to seg0[2]
//set_location_assignment PIN_AE27 -to seg0[1]
//set_location_assignment PIN_AE26 -to seg0[0]
//set_location_assignment PIN_AD27 -to seg1[6]
//set_location_assignment PIN_AF30 -to seg1[5]
//set_location_assignment PIN_AF29 -to seg1[4]
//set_location_assignment PIN_AG30 -to seg1[3]
//set_location_assignment PIN_AH30 -to seg1[2]
//set_location_assignment PIN_AH29 -to seg1[1]
//set_location_assignment PIN_AJ29 -to seg1[0]
//set_location_assignment PIN_AC30 -to seg2[6]
//set_location_assignment PIN_AC29 -to seg2[5]
//set_location_assignment PIN_AD30 -to seg2[4]
//set_location_assignment PIN_AC28 -to seg2[3]
//set_location_assignment PIN_AD29 -to seg2[2]
//set_location_assignment PIN_AE29 -to seg2[1]
//set_location_assignment PIN_AB23 -to seg2[0]
//set_location_assignment PIN_AB22 -to seg3[6]
//set_location_assignment PIN_AB25 -to seg3[5]
//set_location_assignment PIN_AB28 -to seg3[4]
//set_location_assignment PIN_AC25 -to seg3[3]
//set_location_assignment PIN_AD25 -to seg3[2]
//set_location_assignment PIN_AC27 -to seg3[1]
//set_location_assignment PIN_AD26 -to seg3[0]
		