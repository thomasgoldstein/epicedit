
arch snes.cpu

define target $CF0000	//starting location of hack

macro orgl n
    org (({n} & 0x7f0000) >> 1) | ({n} & 0x7fff)
    base {n}
endmacro

macro orgh n
  org {n} & 0x3fffff
  base {n}
endmacro

//////////
//Custom RAM

define colorRam     $7FC000 //256 bytes total (more than enough color #/color pairs)
define vramRam		$7FC100	//2KB (more than enough)

define p2ColorStop	$7FFFE9 //1 byte
define p1ColorStop	$7FFFEA //1 byte
define p2DriverFlag $7FFFEB //1 byte
define p1DriverOld	$7FFFEC //1 byte
define p2DriverOld	$7FFFED //1 byte
define vramRamIndex $7FFFEE //2 bytes
define bg3Scroll    $7FFFF0 //8 bytes (4 16bit values)
define bg3HdmaTable $7FFFF8 //8 bytes (4 entries)

//Original RAM

define players $2e
define frameCounter $34
define mode $36
    define MODE_DRIVERSELECT $06
define p1Driver $66
define p2Driver $68
define p1Confirm $70
define p2Confirm $72
define comflag $0e66

//VRAM

define vramBG1	$4800/2

//////////

{orgh $859234}
	jsl InitPPUDriverSelect

//////////

{orgh $859173}
	jml InitGraphicsRAM

//////////

{orgh $808019}
    jml NMIPart2

//////////

{orgh $85915F}
	nop #5

//uploaded driver select BG1 tilemap (moved to InitGraphicsRAM)
//$85/915F A2 08       LDX #$08                A:0080 X:0002 Y:0000 P:eNvMXdIzc
//$85/9161 20 71 81    JSR $8171  [$85:8171]   A:0080 X:0008 Y:0000 P:envMXdIzc

//////////
//uploaded each of the 256 colors of driver select screen

{orgh $859199}
	nop #8

//85918e lda #$8d00
//859190 sta $2121
//859193 ldy #$0000
//859196 ldx #$e800
//859199 jsr $9ec4	this is called everytime a byte is to be sent
//85919c cpy #$0200
//85919f bne $9196

//////////
//driver select VRAM routines

{orgh $8596d6}
	rts
{orgh $8596dc}
	rts
{orgh $8596e2}
	rts
{orgh $8596ec}
	rts

//////////
//driver select unnecessary tile writers

{orgh $859717}
	rts
{orgh $85971f}
	rts

{orgh $8596f6}
	rts
{orgh $8596fc}
	rts
{orgh $85978f}
	rts

//skips no longer used code

{orgh $8590de}
	jmp $912f

//////////

{orgh $808056}
    jsl Main    //$80/8056 22 CA 09 CF JSL $CF09CA[$CF:09CA]

//////////
//driver sprite positions

{orgh $8592A4}
    incbin "driverpos.bin"

//////////
//BG3 scroll speed

{orgh $85934D}
	dw $0080

//////////

{orgh {target}}

//table of the top left coordinates (tile units) of each driver window
//order is clockwise from the top

DriverPositions:
    db 13,01    //top
    db 21,03
    db 24,11
    db 21,19
    db 13,21    //bottom
    db 05,19
    db 02,11
    db 05,03
    
InitPPUDriverSelect:
	sep #$30
	lda #$01 ; sta $2105	//mode 1 (instead of mode 0)
	lda #$1f ; sta $212c	//all on main screen
	lda #$24 ; sta $2107	//BG1 tilemap
	lda #$28 ; sta $2108	//BG2 tilemap
	lda #$2c ; sta $2109	//BG3 tilemap
	lda #$66 ; sta $210b	//BG1/BG2 tile address
	lda #$01 ; sta $210c	//BG3/BG4 tile address
	lda #$02 ; sta $2101	//object size/location (unchanged?)
	lda #$80 ; sta $2115	//setup VRAM writes for later
	rtl

//////////
//this runs before the NMI waiting code
//return with 16bit AXY

Main:
    sep #$20
    rep #$10
    lda {mode} ; cmp #{MODE_DRIVERSELECT}
    bne .return     //only apply this to driver select
//this is driver select, do all main processing here
    jsr DrawBorderBits
    jsr DrawCursors
    jsr UpdateParralax
    jsr UpdateColors
	jsr UpdatePortraits
	jsr UpdateVRAM
	jsr UpdateOldDrivers	
.return:
    rep #$30
    jml $81E067

UpdateParralax:
    rep #$20
    ldx #$0008-2
.loop:
    lda ^{bg3Scroll},x      //load scroll value to update
    clc ; adc ^.scrollRates,x
    sta ^{bg3Scroll},x      //store updated scroll value
    xba                     //8 bits of fraction
    sta ^{bg3HdmaTable},x   //store new HDMA scroll value
    dex #2 ; bpl .loop

    sep #$20
    rts

.scrollRates:
    dw $100,$0B0,$090

define XOFSLEFT 8
define XOFSRIGHT 32
define YOFS 32
define TILE $06

DrawBorderBits:
    sep #$10
    ldx #<$10-2     //start from end of driver position table
    ldy #$00        //start from sprite 0
.loop:
    lda ^DriverPositions,x      //read xpos
    asl #3                      //*8, it's in tile units
    clc ; adc #<{XOFSLEFT}
    sta $0200,y                 //xpos left tile
    clc ; adc #<{XOFSRIGHT} - {XOFSLEFT}
    sta $0204,y
    lda ^DriverPositions+1,x    //read ypos
    asl #3                      //*8
    clc ; adc #<{YOFS}
    sta $0201,y                 //ypos left tile
    sta $0205,y                 //ypos right tile
    lda #{TILE} ; sta $0202,y ; sta $0206,y
    lda #$b8 ; sta $0203,y      //prop left tile
    lda #$f8 ; sta $0207,y      //prop tile right tile
    tya ; clc ; adc #$08 ; tay  //move to next pair of sprites
    dex #2 ; bpl .loop  

    rep #$10
    rts

define XOFS 16
define P1TILE $00
define P2TILE $02
define COMTILE $04

DrawCursors:
    sep #$10
//draw left side of P1 tile
    lda #{P1TILE}
    ldx {p1Driver}  //load P1 driver selection (it's already in steps of 2)
    ldy #$00
    jsr DrawCursor
//P2 cursor needs drawing if a P2 is present
    lda #{P2TILE}
    ldy {players} ; beq .drawP2
    bit {comflag} ; bpl .return
//COM has a separate cursor
    lda #{COMTILE}
.drawP2:
    ldx {p2Driver}
    ldy #$08
    jsr DrawCursor
.return:
    rep #$10
    rts

//IN:
//A: starting tile #
//X: player index
//Y: OAM index

DrawCursor:
    pha
    lda ^DriverPositions,x
    asl #3              //*8, it's in tile units
    clc ; adc #<{XOFS}  //clc is redundant but w/e
    sta $0240,y         //left xpos
    clc ; adc #$08
    sta $0244,y         //right xpos
    lda ^DriverPositions+1,x
    asl #3              //*8
    sta $0241,y         //left ypos
    sta $0245,y         //right ypos
    pla ; sta $0242,y   //left tile
    inc ; sta $0246,y   //right tile
    lda #$38
    sta $0243,y ; sta $0247,y
    rts

print "UpdateColors: {$}"

UpdateColors:
    phb
    pea $7F7F       //change DB to $7F (write)
    plb ; plb
    ldx #$0000      //reset read index
    ldy #$0000      //reset write index
    bra .test
.loop:
//...test for certain colors
	cmp #$30
	bcs .P2
//must be for P1
	lda >{p1ColorStop} ; bne .skipColor	//don't bother uploading color if it's been flagged to stop
	lda {p1Confirm} ; cmp #$02 ; bne .proceed
	lda {frameCounter} ; and #$3f ; cmp #$14 ; bne .proceed
	lda #$01 ; sta >{p1ColorStop}
	bra .skipColor						//move onto next color
.P2:
	lda >{p2ColorStop} ; bne .skipColor	//don't bother uploading color if it's been flagged to stop
	lda {players} ; beq .testP2
	lda ^{comflag} ; bpl .skipColor
.testP2:
	lda {p2Confirm} ; cmp #$02 ; bne .proceed
	lda {frameCounter} ; and #$3f ; cmp #$14 ; bne .proceed
	lda #$01 ; sta >{p2ColorStop}
	bra .skipColor						//move onto next color	
.proceed:
    rep #$20
    lda {frameCounter}              	//frame counter index
    and #$003f                      	//6bit index for 64 colors
    asl                             	//2 bytes per color
    phx ; clc ; adc $01,s ; tax     	//X += (framecount % 64) * 2
    lda ^.table+1,x                 	//read color in this animation step
    sta >{colorRam}+1,y             	//store color
    iny #3                          	//Y += 2, access next color in RAM table
    pla
	clc ; adc #$0080+1 ; tax  			//128 bytes of color data for each color #
+;  sep #$20
.test:
    lda ^.table,x ; sta >{colorRam},y	//store color # (or 00 terminator)
	bne .loop							//continue if there are more colors
    
    plb
    rts

.skipColor:
	rep #$21
	txa ; adc #$0080+1 ; tax
	bra {-1}

.table:
    incbin "coloranim.bin"

print "UpdateOldDrivers: {$}"

UpdateOldDrivers:
	lda {p1Driver} ; sta {p1DriverOld}
	lda {p2Driver} ; sta {p2DriverOld}
	rts

print "UpdateVRAM: {$}"

define DEFAULTPAL 1
define P1PAL 2
define P2PAL 3

UpdateVRAM:
	phb ; pea $7F7F ; plb ; plb		//switch to VRAM bank
	sep #$10
//update P1
	ldx {p1Driver}
	ldy #<{P1PAL}<<2
	lda >{p1DriverOld} ; cmp {p1Driver} ; beq .drawP1
//player choice changed
	ldx {p1DriverOld}
	ldy #<{DEFAULTPAL}<<2	
.drawP1:		
	jsr UpdateVRAMPlayer			//set blue color for P1
.testP2:
	lda {players} ; beq .updateP2
	lda ^{comflag} ; bpl .return
.updateP2:
	ldx {p2Driver}						//*note if P2 is COM then p2DriverOld is not valid on the first frame
	ldy #<{P2PAL}<<2
	lda ^{p2DriverFlag} ; beq .setP2	//don't draw P2 stuff on the first frame
	lda >{p2DriverOld} ; cmp {p2Driver} ; beq .drawP2
//player choice changed
	ldx {p2DriverOld}
	ldy #<{DEFAULTPAL}<<2
.drawP2:
	jsr UpdateVRAMPlayer
.setP2:
	lda #$01 ; sta ^{p2DriverFlag}	//draw P2
.return:
	sep #$20 ; rep #$10
	plb
	rts

//IN:
//X(8): player index
//Y(8): palette word (high byte)
//DB: assuming $7F
//$00-$05 used

define COLS 6
define ROWS 6

UpdateVRAMPlayer:
	stz $04 ; sty $05				//store palette bits for later use
	lda ^DriverPositions+1,x		//xpos and ypos needed for VRAM calculation
	pha
	lda ^DriverPositions,x ; tax	//X: xpos
	ply								//Y: ypos
	rep #$20
	jsr GetTilemapSource
	pha								//save tilemap source for later
	lsr ; ora #{vramBG1} ; sta $02	//VRAM target
	rep #$10
	plx
	ldy >{vramRamIndex}
	lda #>{ROWS} ; sta $00			//number of rows for loop count
.loop:
	lda #>{COLS}*2 ; sta >{vramRam},y		//byte 0-1: size
	lda $02 ; sta >{vramRam}+2,y			//byte 2-3: target
	clc ; adc #>$20 ; sta $02				//update VRAM target to next row of tilemap
	lda ^BG1Map,x ; and #$e3ff ; ora $04 ; sta >{vramRam}+4,y
	lda ^BG1Map+2,x ; and #$e3ff ; ora $04 ; sta >{vramRam}+6,y
	lda ^BG1Map+4,x ; and #$e3ff ; ora $04 ; sta >{vramRam}+8,y
	lda ^BG1Map+6,x ; and #$e3ff ; ora $04 ; sta >{vramRam}+10,y
	lda ^BG1Map+8,x ; and #$e3ff ; ora $04 ; sta >{vramRam}+12,y
	lda ^BG1Map+10,x ; and #$e3ff ; ora $04 ; sta >{vramRam}+14,y
	txa ; clc ; adc #>$40 ; tax
	tya ; clc ; adc #>{COLS}*2+4 ; tay
	dec $00 ; bne .loop
//terminate list
	lda #$0000 ; sta >{vramRam},y		//0000 terminate list
	tya ; sta >{vramRamIndex}		//update index for next usage
//update P2
	sep #$30
	rts

//IN:
//A(16): VRAM base
//X(8): xpos (tile units)
//Y(8): ypos (tile units)
//OUT:
//A(16): VRAM target

GetVRAMTarget:
	pha
//shift yyyyy bits into position, then xxxxx bits into position
	tya
	asl #5
	sep #$20 ; phx ; ora $01,s ; plx ; rep #$20
	ora $01,s		//combine VRAM base bits
	ply ; ply		//fix stack
	rts

//IN:
//X(8): xpos (tile units)
//Y(8): ypos (tile units)
//OUT:
//A(16): tilemap byte index

//A = (y * 32 + x) * 2 for tilemap byte address

GetTilemapSource:
	tya
	asl #5
	phx ; sep #$20 ; ora $01,s ; plx ; rep #$20
	asl
	rts


define XPOS1PA 13
define YPOS1PA 9
define XPOS1PB 9
define YPOS1PB 9
define XPOS2P 17
define YPOS2P 9

define COLS 6
define ROWS 10

//extend this to support 2P (when COM appears too)

print "UpdatePortraits: {$}"

UpdatePortraits:
	phb ; pea $7f7f ; plb ; plb					//DB set to RAM buffer
	sep #$10 ; rep #$20
//alternate between updating P1/P2 on even/odd frames
	lda {frameCounter} ; and #$0001 ; bne .testP2
//P1 is drawn in a different position if P2 is present...
	ldx #<{XPOS1PB}
	ldy #<{YPOS1PB}
	lda {players} ; beq .updateP1
	lda ^{comflag} ; bmi .updateP1
//P1 is drawn in the center if P2 is absent
	ldx #<{XPOS1PA}
	ldy #<{YPOS1PA}	
.updateP1:
	phx
//24bit P1 driver pointer stored to $00-$02
	lda {p1Driver} ; lsr ; adc {p1Driver} ; tax		
	lda ^.pointers,x ; sta $00 ; lda ^.pointers+1,x ; sta $01
	plx
	lda #>{COLS} ; sta $03
	lda #>{ROWS} ; sta $05
	jsr UpdateMap
.testP2:
	lda {players} ; beq .updateP2
	lda ^{comflag} ; bpl .return
	lda ^{p2DriverFlag} ; and #$00ff ; bne .updateP2
//P1 clear (when COM is introduced)
	ldx #<{XPOS1PA}
	ldy #<{YPOS1PA}
	lda #>{COLS} ; sta $03
	lda #>{ROWS} ; sta $05
	jsr UpdateMapClear		
	bra .return
.updateP2:
	//24bit P2 mirrored driver pointer stored to $00-$02
	lda {p2Driver} ; lsr ; adc {p2Driver} ; tax		
	lda ^.pointersMirrored,x ; sta $00 ; lda ^.pointersMirrored+1,x ; sta $01
	ldx #<{XPOS2P}
	ldy #<{YPOS2P}
	lda #>{COLS} ; sta $03
	lda #>{ROWS} ; sta $05
	jsr UpdateMap	
.return:
	rep #$10 ; sep #$20
	plb
	rts

//regular
.pointers:
	dl .d0,.d1,.d2,.d3,.d4,.d5,.d6,.d7
.d0:
	incbin "portraits\0.bin"
.d1:
	incbin "portraits\1.bin"
.d2:
	incbin "portraits\2.bin"
.d3:
	incbin "portraits\3.bin"
.d4:
	incbin "portraits\4.bin"
.d5:
	incbin "portraits\5.bin"
.d6:
	incbin "portraits\6.bin"
.d7:
	incbin "portraits\7.bin"
//mirrored
.pointersMirrored:
	dl .d0m,.d1m,.d2m,.d3m,.d4m,.d5m,.d6m,.d7m
.d0m:
	incbin "portraits\0m.bin"
.d1m:
	incbin "portraits\1m.bin"
.d2m:
	incbin "portraits\2m.bin"
.d3m:
	incbin "portraits\3m.bin"
.d4m:
	incbin "portraits\4m.bin"
.d5m:
	incbin "portraits\5m.bin"
.d6m:
	incbin "portraits\6m.bin"
.d7m:
	incbin "portraits\7m.bin"

//IN:
//A(16): ...
//X(8): xpos (tile units)
//Y(8): ypos (tile units)
//$00-$02: pointer to tile data to upload
//$03-$04: cols
//$05-$06: rows
//$07-$08: temp cols
//$09-$0a: temp rows
//$0b-$0c: temp vram
UpdateMap:
	jsr GetTilemapSource
	rep #$10
	lsr ; ora #{vramBG1} ; sta $0b				//VRAM target of portrait
	ldx >{vramRamIndex}						
	ldy #$0000
	lda $05 ; sta $09							//loop counter rows
.loopY:
	lda $03 ; asl ; sta >{vramRam},x			//1: size set (for this row)
	lda $0b ; sta >{vramRam}+2,x				//2: vram set (for this row)
	inx #4
	clc ; adc #$0020 ; sta $0b					//move VRAM to next row						
	lda $03 ; sta $07							//loop counter columns
.loopX:
	lda [$00],y ; sta >{vramRam},x
	inx #2 ; iny #2
	dec $07 ; bne .loopX			//for every column...
	dec $09 ; bne .loopY			//for every row...
//terminate list
	lda #$0000 ; sta >{vramRam},x
	txa ; sta >{vramRamIndex}
	sep #$10
	rts

//IN:
//A(16): ...
//X(8): xpos (tile units)
//Y(8): ypos (tile units)
//$03-$04: cols
//$05-$06: rows
//$07-$08: temp cols
//$09-$0a: temp rows
//$0b-$0c: temp vram

define CLEARTILE $000F

UpdateMapClear:
	jsr GetTilemapSource
	rep #$10
	lsr ; ora #{vramBG1} ; sta $0b				//VRAM target of portrait
	ldx >{vramRamIndex}						
	lda $05 ; sta $09							//loop counter rows
.loopY:
	lda $03 ; asl ; sta >{vramRam},x			//1: size set (for this row)
	lda $0b ; sta >{vramRam}+2,x				//2: vram set (for this row)
	inx #4
	clc ; adc #$0020 ; sta $0b					//move VRAM to next row						
	lda $03 ; sta $07							//loop counter columns
	lda #{CLEARTILE}
.loopX:
	sta >{vramRam},x
	inx #2 ; iny #2
	dec $07 ; bne .loopX			//for every column...
	dec $09 ; bne .loopY			//for every row...
//terminate list
	lda #$0000 ; sta >{vramRam},x
	txa ; sta >{vramRamIndex}
	sep #$10
	rts

//////////

NMIPart2:
    sep #$20
    rep #$10
    lda {mode} ; cmp #{MODE_DRIVERSELECT}
    bne .return     //only apply this to driver select (could probably be put inside the JSR jump table instead)
//this is driver select, do any video stuff here
    jsr UploadColors
	jsr UploadVRAM
    jsr SetupHDMA   //BG3 scrolling clouds
.return:
    rep #$38        //back to smk
    ply
    plx                   
    pla                     
    plb                     
    plp                     
    jml $80801E     //to RTI 

print "UploadColors: {$}"

UploadColors:
    pea $807F ; plb     //DB = bank where color buffer is
    sep #$10
    pea $2100 ; pld
    ldx #$00            //note this limits color table to 256 bytes total
    bra .test
.loop:
    sta <$2121
    lda >{colorRam}+1,x ; sta <$2122
    lda >{colorRam}+2,x ; sta <$2122
    inx #3
.test:
    lda >{colorRam},x ; bne .loop

    xba ; lda #$00 ; tad
    rep #$10
    plb
    rts

print "UploadVRAM: {$}"

UploadVRAM:
	lda #$80 ; sta $2115
	lda #$01 ; sta $4300
	lda #$18 ; sta $4301			//upload words to $2118
	lda #<{vramRam}>>16 ; sta $4304	//source bank is always in RAM
	ldx #>{vramRam}					//X: start of list in RAM bank
	rep #$21
	lda #$4300 ; tad
	bra .test
.loop:
	sta <$4305
	lda $7F0002,x ; sta $2116
	txa ; adc #>4 ; sta <$4302
	sep #$20 ; lda #$01 ; sta $420b ; rep #$20
	ldx <$4302
.test:
	lda $7F0000,x ; bne .loop			//byte 0-1: size (0000 terminates the loop)
	
	sta ^{vramRamIndex}					//don't repeat transfer next frame
	lda #$0000 ; tad
	sep #$20
	rts

SetupHDMA:
//parralax scroll
    lda #$42 ; sta $4310    //indirect write twice to scroll register
    lda #$0f ; sta $4311    //BG3 xscroll
    ldx #.table ; stx $4312
    lda #<.table>>16 ; sta $4314
    lda #<{bg3HdmaTable}>>16 ; sta $4317
//mountain coloring
	stz $4350 ; stz $4360 ; stz $4370				//direct writes, no variable color data
	lda #$32 ; sta $4351 ; sta $4361 ; sta $4371	//$2132 RGB targets
	ldx #.tableR ; stx $4352
	ldx #.tableG ; stx $4362
	ldx #.tableB ; stx $4372
	lda #<.tableR>>16 ; sta $4354
	lda #<.tableG>>16 ; sta $4364
	lda #<.tableB>>16 ; sta $4374
//subscreen/main screen setting
	lda #$01 ; sta $4320
	lda #$2c ; sta $4321
	ldx #.tableScreen ; stx $4322
	lda #<.tableScreen>>16 ; sta $4324
//cgadsub setting
	stz $4330
	lda #$31 ; sta $4331
	ldx #.tableCG ; stx $4332
	lda #<.tableCG>>16 ; sta $4334
//CH1,5,6,7
    lda #$ee ; sta $420c
//color math setup
	lda #$02 ; sta $2130
    rts

.table:
    db $30-1
    dw {bg3HdmaTable}
    db $30
    dw {bg3HdmaTable}+2
    db $10
    dw {bg3HdmaTable}+4
    db 0
.tableR:
	incbin "hdmaRed.bin"
.tableG:
	incbin "hdmaGreen.bin"
.tableB:
	incbin "hdmaBlue.bin"
.tableScreen:
	db $7F,$1D,$02,$1F,$1D,$02,$01,$1F,$00
	db 0
.tableCG:
	db $7F,$64,$1F,$64,$01,$02
	db 0
//////////

InitGraphicsRAM:
	rep #$10
	sep #$20
//palette
	stz $2121
	stz $4300
	lda #$22 ; sta $4301
	ldx #.mainPal ; stx $4302
	lda #<.mainPal>>16 ; sta $4304
	ldx #$0200 ; stx $4305
	lda #$01 ; sta $420b
//main driver select GFX (16KB @ $C000)
	lda #$01 ; sta $4300
	lda #$18 ; sta $4301
	ldx #$6000 ; stx $2116
	ldx #.mainGFX ; stx $4302
	lda #<.mainGFX>>16 ; sta $4304
	ldx #$4000 ; stx $4305
	lda #$01 ; sta $420b
//BG3 tiles (8KB @ $2000)
	ldx #$1000 ; stx $2116
	ldx #.bg3gfx ; stx $4302
	lda #<.bg3gfx>>16 ; sta $4304
	ldx #$2000 ; stx $4305
	lda #$01 ; sta $420b
//sprite tiles (256B @ $8000)
    ldx #$4000 ; stx $2116
    ldx #.spritegfx ; stx $4302
    lda #<.spritegfx>>16 ; sta $4304
    ldx #$0100 ; stx $4305
    lda #$01 ; sta $420b
//BG1 tilemap (2KB @ $4800)
	ldx #$2400 ; stx $2116
	ldx #BG1Map ; stx $4302
	lda #<BG1Map>>16 ; sta $4304
	ldx #$0800 ; stx $4305
	lda #$01 ; sta $420b
//BG2 tilemap (2KB @ $5000)
	ldx #$2800 ; stx $2116
	ldx #.bg2map ; stx $4302
	lda #<.bg2map>>16 ; sta $4304
	ldx #$0800 ; stx $4305
	lda #$01 ; sta $420b
//BG3 tilemap (2KB @ $5800)
	ldx #$2c00 ; stx $2116
	ldx #.bg3map ; stx $4302
	lda #<.bg3map>>16 ; sta $4304
	ldx #$0800 ; stx $4305
	lda #$01 ; sta $420b
//non-graphical initialisation (move somewhere else?)
//don't do any transfers at first    
    lda #$00 ; sta ^{colorRam} ; sta ^{vramRam} ; sta ^{vramRam}+1 ; sta ^{p1ColorStop} ; sta ^{p2ColorStop}
	sta ^{p2DriverFlag}

	rep #$30		//back to SMK
	//...
	lda #$3800
	jml $859178		//$CF/0C32 5C 78 91 85 JMP $859178[$85:9178]

.mainGFX:
	incbin "playsel_gfx.bin"
.bg3gfx:
	incbin "bg3gfx.bin"
.spritegfx:
    incbin "spritegfx.bin"
.bg2map:
	incbin "bg2map.bin"
.bg3map:
	incbin "bg3map.bin"
.mainPal:
	incbin "pal.bin"

//the below must be global

BG1Map:
	incbin "bg1map.bin"