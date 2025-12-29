////////////////////////////////////////////////////////////////////////////////
//
// Engineer: Hudson Dalby
//
// Create Date:   2025-09-15
// Board Name:    Fibonacci Sequence Board
// Dependencies:  processor.v, sevSeg_16b.v
// Description:   FPGA Board demonstration of fibonacci sequence. Cycles through
// 		  each of the 16 registers with an FSM. 
//
////////////////////////////////////////////////////////////////////////////////
module fibonacci_board (
	input wire clk,
	input wire reset,

	output wire [6:0] hex0,
	output wire [6:0] hex1, 
	output wire [6:0] hex2,
	output wire [6:0] hex3
); 

	// Processor control 
	reg [3:0] op;
	reg [15:0] regEnable, immediate;
	reg [3:0] sourceReg, destReg;
	reg flagWrite;
	reg immControl;


	// Processor outputs 
	wire [15:0] O;
	wire [4:0] flags;
	wire [15:0] r0, r1, r2, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r13, r14, r15;

	// ALU op codes 
	parameter p_MOVE = 4'b0001;
	parameter p_ADD  = 4'b0100;

	// FSM States
	parameter   S0=4'd0, S1=4'd1, S2=4'd2, S3=4'd3, S4=4'd4, S5=4'd5, S6=4'd6, S7=4'd7,
				S8=4'd8, S9=4'd9, S10=4'd10, S11=4'd11, S12=4'd12, S13=4'd13, P0 = 4'd14, P1 = 4'd15;
				
	reg [3:0] state;

	// Processor test module
	processor uut(
		.op(op),
		.flags(flags),
		.reset(reset),
		.clk(clk),
		.regEnable(regEnable),
		.r0(r0), 
		.r1(r1), 
		.r2(r2), 
		.r3(r3), 
		.r4(r4), 
		.r5(r5), 
		.r6(r6), 
		.r7(r7), 
		.r8(r8), 
		.r9(r9), 
		.r10(r10), 
		.r11(r11), 
		.r12(r12), 
		.r13(r13), 
		.r14(r14), 
		.r15(r15),
		.sourceReg(sourceReg),
		.destReg(destReg),
		.immControl(immControl),
		.immediate(immediate),
		.flagWrite(flagWrite)
	);

	// Shows the value of r15 on the display. 
	sevSeg_16b display(
		.value(r15),
		.seg3(hex3),
		.seg2(hex2),
		.seg1(hex1),
		.seg0(hex0)
	);


	// State machine flow
	always @(posedge clk) 
	begin
		if (!reset) 
		begin

			state <= P0;

		end 
		else 
		begin

			case(state)
				P0: state <= P1;
				P1: state <= S0;
				S0: state <= S1;
				S1: state <= S2;
				S2: state <= S3;
				S3: state <= S4;
				S4: state <= S5;
				S5: state <= S6;
				S6: state <= S7;
				S7: state <= S8;
				S8: state <= S9;
				S9: state <= S10;
				S10: state <= S11;
				S11: state <= S12;
				S12: state <= S13;
				S13: state <= S13;
				default: state <= state;
			endcase

		end
	end


	always @(posedge clk) begin

		if (!reset) begin

			op         <= p_ADD; 
			sourceReg  <= 4'd0;
			destReg    <= 4'd0;
			regEnable  <= 16'b0; 
			flagWrite  <= 1'b0;   
			immControl <= 1'b0;
			immediate  <= 16'd0;

		end 
		else 
		begin

			op         <= p_ADD;
			sourceReg  <= 4'd0;
			destReg    <= 4'd0;
			regEnable  <= 16'b0;
			flagWrite  <= 1'b0;
			immControl <= 1'b0;
			immediate  <= 16'd0;

			case (state) 

				P0: begin
				op <= p_MOVE;
				immControl <= 1'b1;
				immediate <= 16'd0;
				destReg <= 4'd1;
				regEnable <= 16'b0000_0000_0000_0001;
				end

				P1: begin
				op <= p_MOVE;
				immControl <= 1'b1;
				immediate <= 16'd1;
				destReg <= 4'd1;
				regEnable <= 16'b0000_0000_0000_0010;
				end

				// S0: r0 + r1 -> r2
				S0: begin
				op <= p_ADD;
				sourceReg <= 4'd0;
				destReg <= 4'd1;
				regEnable <= 16'b0000_0000_0000_0100;
				flagWrite <= 1'b1; 
				end


				// S1: r1 + r2 -> r3
				S1: begin
				op <= p_ADD;
				sourceReg <= 4'd1;
				destReg <= 4'd2;
				regEnable <= 16'b0000_0000_0000_1000;
				flagWrite <= 1'b1;
				end

				// S2: r2 + r3 -> r4
				S2: begin
				op <= p_ADD;
				sourceReg <= 4'd2;
				destReg <= 4'd3;
				regEnable <= 16'b0000_0000_0001_0000;
				flagWrite <= 1'b1;
				end

				// S3: r3 + r4 -> r5
				S3: begin
				op <= p_ADD;
				sourceReg <= 4'd3;
				destReg <= 4'd4;
				regEnable <= 16'b0000_0000_0010_0000;
				flagWrite <= 1'b1;
				end

				// S4: r4 + r5 -> r6
				S4: begin
				op <= p_ADD;
				sourceReg <= 4'd4;
				destReg <= 4'd5;
				regEnable <= 16'b0000_0000_0100_0000;
				flagWrite <= 1'b1;
				end

				// S5: r5 + r6 -> r7
				S5: begin
				op <= p_ADD;
				sourceReg <= 4'd5;
				destReg <= 4'd6;
				regEnable <= 16'b0000_0000_1000_0000;
				flagWrite <= 1'b1;
				end

				// S6: r6 + r7 -> r8
				S6: begin
				op <= p_ADD;
				sourceReg <= 4'd6;
				destReg <= 4'd7;
				regEnable <= 16'b0000_0001_0000_0000;
				flagWrite <= 1'b1;
				end

				// S7: r7 + r8 -> r9
				S7: begin
				op <= p_ADD;
				sourceReg <= 4'd7;
				destReg <= 4'd8;
				regEnable <= 16'b0000_0010_0000_0000;
				flagWrite <= 1'b1;
				end

				// S8: r8 + r9 -> r10
				S8: begin
				op <= p_ADD;
				sourceReg <= 4'd8;
				destReg <= 4'd9;
				regEnable <= 16'b0000_0100_0000_0000;
				flagWrite <= 1'b1;
				end

				// S9: r9 + r10 -> r11
				S9: begin
				op <= p_ADD;
				sourceReg <= 4'd9;
				destReg <= 4'd10;
				regEnable <= 16'b0000_1000_0000_0000;
				flagWrite <= 1'b1;
				end

				// S10: r10 + r11 -> r12
				S10: begin
				op <= p_ADD;
				sourceReg <= 4'd10;
				destReg <= 4'd11;
				regEnable <= 16'b0001_0000_0000_0000;
				flagWrite <= 1'b1;
				end

				// S11: r11 + r12 -> r13
				S11: begin
				op <= p_ADD;
				sourceReg <= 4'd11;
				destReg <= 4'd12;
				regEnable <= 16'b0010_0000_0000_0000;
				flagWrite <= 1'b1;
				end

				// S12: r12 + r13 -> r14
				S12: begin
				op <= p_ADD;
				sourceReg <= 4'd12;
				destReg <= 4'd13;
				regEnable <= 16'b0100_0000_0000_0000;
				flagWrite <= 1'b1;
				end

				// S13: r13 + r14 -> r15
				S13: begin
				op <= p_ADD;
				sourceReg <= 4'd13;
				destReg <= 4'd14;
				regEnable <= 16'b1000_0000_0000_0000;
				flagWrite <= 1'b1;
				end

			endcase

		end
	end
endmodule


