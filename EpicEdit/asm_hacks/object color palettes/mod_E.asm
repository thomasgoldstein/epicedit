hirom

;where this hack will be inserted in ROM
!target			= $CF0000
!objects_routine	= $C80030

;routines for each region:
!piper1_U	= $91D0
!piper2_U	= $E4E7
!piper3_U	= $E4F7
!piper1_J	= $91E4
!piper2_J	= $E50C
!piper3_J	= $E51C
!piper1_E	= $91E9
!piper2_E	= $E50C
!piper3_E	= $E51C
!pillarr1_U	= $91D0
!pillarr2_U	= $E6FE
!pillarr3_U	= $E708
!pillarr1_J	= $91E4
!pillarr2_J	= $E704
!pillarr3_J	= $E70E
!pillarr1_E	= $91E9
!pillarr2_E	= $E723
!pillarr3_E	= $E72D
!thwompr1_U	= $91D0
!thwompr2_U	= $E18B
!thwompr3_U	= $E19D
!thwompr1_J	= $91E4
!thwompr2_J	= $E1B0
!thwompr3_J	= $E1C2
!thwompr1_E	= $91E9
!thwompr2_E	= $E1B0
!thwompr3_E	= $E1C2
!moler1_U	= $91E4
!moler2_U	= $E36B
!moler3_U	= $E37B
!moler1_J	= $91F8
!moler2_J	= $E390
!moler3_J	= $E3A0
!moler1_E	= $91FD
!moler2_E	= $E390
!moler3_E	= $E3A0
!plantr1_U	= $91D0
!plantr2_U	= $E06E
!plantr3_U	= $E07A
!plantr1_J	= $91E4
!plantr2_J	= $E093
!plantr3_J	= $E09F
!plantr1_E	= $91E9
!plantr2_E	= $E093
!plantr3_E	= $E09F
!fishr1_U	= $91C0
!fishr2_U	= $DF7E
!fishr3_U	= $DF8E
!fishr1_J	= $91D4
!fishr2_J	= $DFA3
!fishr3_J	= $DFB3
!fishr1_E	= $91D9
!fishr2_E	= $DFA3
!fishr3_E	= $DFB3
!rthwompr1_U	= $91D0
!rthwompr2_U	= $E144
!rthwompr3_U	= $E156
!rthwompr1_J	= $91E4
!rthwompr2_J	= $E169
!rthwompr3_J	= $E17B
!rthwompr1_E	= $91E9
!rthwompr2_E	= $E169
!rthwompr3_E	= $E17B

;routines for this ROM:
!piper1		= !piper1_E
!piper2		= !piper2_E
!piper3		= !piper3_E
!pillarr1	= !pillarr1_E
!pillarr2	= !pillarr2_E
!pillarr3	= !pillarr3_E
!thwompr1	= !thwompr1_E
!thwompr2	= !thwompr2_E
!thwompr3	= !thwompr3_E
!moler1		= !moler1_E
!moler2		= !moler2_E
!moler3		= !moler3_E
!plantr1	= !plantr1_E
!plantr2	= !plantr2_E
!plantr3	= !plantr3_E
!fishr1		= !fishr1_E
!fishr2		= !fishr2_E
!fishr3		= !fishr3_E
!rthwompr1	= !rthwompr1_E
!rthwompr2	= !rthwompr2_E
!rthwompr3	= !rthwompr3_E

!molegfx_U	= $80E44E
!molegfx_J	= $80E473
!molegfx_E	= $80E47E
!molegfx	= !molegfx_E

;object indexes:
;$04: routine 2

!framecounter		= $38
!course			= $0124
!current_object	= $B4	;current index (into $0000-$1FFF) of current object
	!oi_routine2	= $04

;---

!PW1org_U	= $C0BD0E
!PW1org_J	= $C0BD33
!PW1org_E	= $C0BD33
!PW1org		= !PW1org_E

;U:
;$80/BD0E B9 06 00    LDA $0006,y[$80:E5C2]
;$80/BD11 45 1A       EOR $1A    [$00:001A]	;$1A is applied to each tile # prop byte
;$80/BD13 95 0E       STA $0E,x  [$00:027E]	;tile 3
;$80/BD15 B9 04 00    LDA $0004,y[$80:E5C0]
;$80/BD18 45 1A       EOR $1A    [$00:001A]
;$80/BD1A 95 0A       STA $0A,x  [$00:027A]	;tile 2
;$80/BD1C B9 02 00    LDA $0002,y[$80:E5BE]
;$80/BD1F 45 1A       EOR $1A    [$00:001A]
;$80/BD21 95 06       STA $06,x  [$00:0276]	;tile 1
;$80/BD23 B9 00 00    LDA $0000,y[$80:E5BC]
;$80/BD26 45 1A       EOR $1A    [$00:001A]
;$80/BD28 95 02       STA $02,x  [$00:0272]	;tile 0
;$80bd2a txa
;J:
;80bd33 lda $0006,y
;80bd36 eor $1a
;80bd38 sta $0e,x
;80bd3a lda $0004,y
;80bd3d eor $1a
;80bd3f sta $0a,x
;80bd41 lda $0002,y
;80bd44 eor $1a
;80bd46 sta $06,x
;80bd48 lda $0000,y
;80bd4b eor $1a
;80bd4d sta $02,x
;80bd4f txa

org !PW1org
	jml	PaletteWrite1	;32x32

!PW1ret_U	= $80BD2A
!PW1ret_J	= $80BD4F
!PW1ret_E	= $80BD4F
!PW1ret		= !PW1ret_E

;---

!PW2org_U	= $C0BD86
!PW2org_J	= $C0BDAB
!PW2org_E	= $C0BDAB
!PW2org		= !PW2org_E

;U:
;80bd86 lda $0002,x   [80e60c] A:0001 X:e60a Y:bd46 S:1fed D:0000 DB:80 Nvmxdizc V: 16 H:1340
;80bd89 ldx $3c       [00003c] A:2ee8 X:e60a Y:bd46 S:1fed D:0000 DB:80 nvmxdizc V: 17 H:  12
;80bd8b eor $1a       [00001a] A:2ee8 X:0280 Y:bd46 S:1fed D:0000 DB:80 nvmxdizc V: 17 H:  40
;80bd8d sta $02,x     [000282] A:2ee8 X:0280 Y:bd46 S:1fed D:0000 DB:80 nvmxdizc V: 17 H:  68
;80bd8f lda $0000,y   [80bd46] A:2ee8 X:0280 Y:bd46 S:1fed D:0000 DB:80 nvmxdizc V: 17 H: 102
;J:
;80bdab lda $0002,x
;80bdae ldx $3c
;80bdb0 eor $1a
;80bdb2 sta $02,x
;80bdb4 lda $0000,y

org !PW2org
	jml	PaletteWrite2	;16x16

!PW2ret_U	= $80BD8F
!PW2ret_J	= $80BDB4
!PW2ret_E	= $80BDB4
!PW2ret		= !PW2ret_E

;---

!PW3org_U	= $C0BD1C
!PW3org_J	= $C0BD41
!PW3org_E	= $C0BD41
!PW3org		= !PW3org_E

;U:
;80bc7b jmp $bd1c
;80bd1c lda $0002,y   [80e460] A:45e0 X:02a0 Y:e45e S:1fed D:0000 DB:80 Nvmxdizc V: 24 H:1044
;80bd1f eor $1a       [00001a] A:2eca X:02a0 Y:e45e S:1fed D:0000 DB:80 nvmxdizc V: 24 H:1080
;80bd21 sta $06,x     [0002a6] A:2eca X:02a0 Y:e45e S:1fed D:0000 DB:80 nvmxdizc V: 24 H:1108
;80bd23 lda $0000,y   [80e45e] A:2eca X:02a0 Y:e45e S:1fed D:0000 DB:80 nvmxdizc V: 24 H:1292
;80bd26 eor $1a       [00001a] A:2ec8 X:02a0 Y:e45e S:1fed D:0000 DB:80 nvmxdizc V: 24 H:1328
;80bd28 sta $02,x     [0002a2] A:2ec8 X:02a0 Y:e45e S:1fed D:0000 DB:80 nvmxdizc V: 24 H:1356
;80bd2a txa                    A:2ec8 X:02a0 Y:e45e S:1fed D:0000 DB:80 nvmxdizc V: 25 H:  26
;J:
;;80bd41 lda $0002,y
;80bd44 eor $1a
;80bd46 sta $06,x
;80bd48 lda $0000,y
;80bd4b eor $1a
;80bd4d sta $02,x
;80bd4f txa

org !PW3org
	jml	PaletteWrite3	;16x32/32x16

!PW3ret_U	= $80BD2A
!PW3ret_J	= $80BD4F
!PW3ret_E	= $80BD4F
!PW3ret		= !PW3ret_E

;---

!DP1org_U	= $C0E4D6
!DP1org_J	= $C0E4FB
!DP1org_E	= $C0E4FB
!DP1org		= !DP1org_E

;U:
;$80/E4D0 AD 24 01    LDA $0124  [$80:0124]   A:0000 X:E4ED Y:E4EB P:eNvmxdizc
;$80/E4D3 C9 13 00    CMP #$0013              A:0007 X:E4ED Y:E4EB P:envmxdizc
;$80/E4D6 D0 0A       BNE $0A    (+)      A:0007 X:E4ED Y:E4EB P:eNvmxdizc
;J:
;80e4f5 lda $0124
;80e4f8 cmp #$0013
;80e4fb bne $e507

org !DP1org
	db $80

;---

;U:
;80e171 lda $38		;frame counter
;80e173 and #$0003
;80e176 asl a
;80e177 tay
;80e178 lda $e183,y
;80e17b sta $1a
;80e17d plx

;---

org !target

print "palettes: ", pc

Palettes:
	db $0E,0,0,0		;MC3
	db $0E,0,0,0		;GV2
	db $0E,0,0,0		;DP2
	db $08,0,0,0		;BC2
	db $0E,0,0,0		;VL2
	db $02,$0E,$08,$0E	;RR
	db $0C,0,0,0		;KB2
	db $0E,0,0,0		;MC1
	db $0E,0,0,0		;GV3
	db $08,0,0,0		;BC3
	db $0C,0,0,0		;CI2
	db $0E,0,0,0		;DP3
	db $0E,0,0,0		;VL1
	db $0C,0,0,0		;KB1
	db $0E,0,0,0		;MC4
	db $0E,0,0,0		;MC2
	db $0E,0,0,0		;GV1
	db $08,0,0,0		;BC1
	db $0C,0,0,0		;CI1
	db $0A,0,0,0		;DP1
;...
	db 0,0,0,0
	db 0,0,0,0
	db 0,0,0,0
	db 0,0,0,0

;6EC2

print "palette cycle flags: ", pc

PaletteCycleFlags:
	db 0	;MC3
	db 0	;GV2
	db 0	;DP2
	db 0	;BC2
	db 0	;VL2
	db 1	;RR
	db 0	;KB2
	db 0	;MC1
	db 0	;GV3
	db 0	;BC3
	db 0	;CI2
	db 0	;DP3
	db 0	;VL1
	db 0	;KB1
	db 0	;MC4
	db 0	;MC2
	db 0	;GV1
	db 0	;BC1
	db 0	;CI1
	db 0	;DP1
;...
	db 0,0,0,0

;---

;16bit AXY
;preserve XY

print "pw1: ", pc

PaletteWrite1:
	jsr	CheckObject	;does this object need a palette change?
	bne	.normal
.change
	pei	($00)
	jsr	GetCoursePalette
	lda	$0006,y		;tile 3
	eor	$1A
	and	#$F1FF		;mask out old palette bits
	ora	$00		;set new palette bits
	sta	$0E,x
	lda	$0004,y		;tile 2
	eor	$1A
	and	#$F1FF		;mask out old palette bits
	ora	$00		;set new palette bits
	sta	$0A,x
	lda	$0002,y		;tile 1
	eor	$1A
	and	#$F1FF		;mask out old palette bits
	ora	$00		;set new palette bits
	sta	$06,x
	lda	$0000,y		;tile 0
	eor	$1A
	and	#$F1FF		;mask out old palette bits
	ora	$00		;set new palette bits
	sta	$02,x
	pla : sta $00
	jml	!PW1ret
.normal
	lda	$0006,y		;tile 3
	eor	$1A
	sta	$0E,x
	lda	$0004,y		;tile 2
	eor	$1A
	sta	$0A,x
	lda	$0002,y		;tile 1
	eor	$1A
	sta	$06,x
	lda	$0000,y		;tile 0
	eor	$1A
	sta	$02,x
	jml	!PW1ret

;---

;16bit AXY
;preserve XY

print "pw2: ", pc

PaletteWrite2:
	jsr	CheckObject	;does this object need a palette change?
	bne	.normal
.change
	pei	($00)
	jsr	GetCoursePalette
	lda	$0002,x		;tile 0
	ldx	$3C
	eor	$1A
	and	#$F1FF		;mask out old palette bits
	ora	$00		;set new palette bits
	sta	$02,x
	pla : sta $00
	jml	!PW2ret
.normal
	lda	$0002,x		;tile 0
	ldx	$3C
	eor	$1A
	sta	$02,x
	jml	!PW2ret
	
;---

;16bit AXY
;preserve XY

print "pw3: ", pc

PaletteWrite3:
	jsr	CheckObject	;does this object need a palette change?
	bne	.normal
.change
	pei	($00)
	jsr	GetCoursePalette
	lda	$0002,y		;tile 1
	eor	$1A
	and	#$F1FF		;mask out old palette bits
	ora	$00		;set new palette bits
	sta	$06,x
	lda	$0000,y		;tile 0
	eor	$1A
	and	#$F1FF		;mask out old palette bits
	ora	$00		;set new palette bits
	sta	$02,x
	pla : sta $00
	jml	!PW3ret
.normal
	lda	$0002,y		;tile 1
	eor	$1A
	sta	$06,x
	lda	$0000,y		;tile 0
	eor	$1A
	sta	$02,x
	jml	!PW3ret

;---

;out:
;Z: set if object needs to be modded

print "co: ", pc

CheckObject:
	phx
	ldx	!current_object
	lda	!oi_routine2,x		;load current object routine
	ldx	!course
	pha
	lda.l	!objects_routine,x
	and	#$00FF
	asl	a
	tax
	pla
	cmp.l	.routines2,x		;current routine is main view routine
	beq	+
	cmp.l	.routines3,x		;current routine is rear-view routine?
	beq	+
	cpy.w	#!molegfx		;is this a mole attached to kart?
	beq	+
	plx
	clc		
	lda	#$0001
	rts
+	clc
	plx
	clc
	lda	#$0000
	rts
.routines2
	dw !piper2	;PIPE
	dw !pillarr2	;PILLAR
	dw !thwompr2	;THWOMP
	dw !moler2	;MOLE
	dw !plantr2	;PLANT
	dw !fishr2	;FISH
	dw !rthwompr2	;RTHWOMP
.routines3
	dw !piper3	;PIPE
	dw !pillarr3	;PILLAR
	dw !thwompr3	;THWOMP
	dw !moler3	;MOLE
	dw !plantr3	;PLANT
	dw !fishr3	;FISH
	dw !rthwompr3	;RTHWOMP

;---

;loads $00-$01 with palette bits

print "getcp: ", pc

GetCoursePalette:
	phx
	ldx	!course
	sep	#$20
	lda.l	PaletteCycleFlags,x	;is this course set to palette cycle?
	beq	.noCycle
.cycle
	txa
	asl	#2		;4 palettes per course
	sta	$00
	lda	!framecounter	;low 2 bits of frame counter decides which palette to use
	and	#$03
	clc : adc $00		;add value 0-3 based on frame of animation
	bra	.read
.noCycle
	txa
	asl	#2		;4 palettes per course, no cycling
.read
	tax
	lda.l	Palettes,x
	rep	#$20
	and	#$00FF
	xba
	sta	$00
	plx
	rts