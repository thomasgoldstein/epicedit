hirom

;where this hack will be inserted in ROM
!target	= $C80000

!BATTLEBASE	= $14

;ram:
!course			= $0124
!object_interact	= $0E30
!object_thing		= $0D28
!object_zbase		= $0FFC
!object_zsize		= $0FFE

;rom:
!object_gfx_ptrs	= $81EBD3
!object_zbase_table	= $818BCD
!object_zsize_table	= $818BDD

;original theme defs
!THEME_GV	= $00
!THEME_MC	= $02
!THEME_DP	= $04
!THEME_CI	= $06
!THEME_VL	= $08
!THEME_KB	= $0A
!THEME_BC	= $0C
!THEME_RR	= $0E

;new loader defs
!LOADER_NORM	= $00
!LOADER_FISH	= $01
!LOADER_GV	= $02
!LOADER_NONE	= $03

;$0E30 setting, GFX pointer setting
;$81/E992 BC D3 EB    LDY $EBD3,x[$81:EBD6]   A:0003 X:0003 Y:0007 P:eNvmxdIzc	;grab 24bit pointer based on theme
;$81/E995 BD D5 EB    LDA $EBD5,x[$81:EBD8]   A:0003 X:0003 Y:0F9B P:envmxdIzc
;$81/E998 29 FF 00    AND #$00FF              A:D6C1 X:0003 Y:0F9B P:eNvmxdIzc

org $C1E992
	jml	ObjectSettingA
	
;$81/8EDF B9 CD 8B    LDA $8BCD,y[$81:8BCF]   A:0007 X:0002 Y:0002 P:envmxdIzc	;*track based data loaded here
;$81/8EE2 8D FC 0F    STA $0FFC  [$81:0FFC]   A:0000 X:0002 Y:0002 P:envmxdIZc	;*zpos base
;$81/8EE5 B9 DD 8B    LDA $8BDD,y[$81:8BDF]   A:0000 X:0002 Y:0002 P:envmxdIZc
;$81/8EE8 8D FE 0F    STA $0FFE  [$81:0FFE]   A:0100 X:0002 Y:0002 P:envmxdIzc	;*zpos size,defines how 'tall' the object interaction field is? (try thwomps)
;818eeb ldy #$ee00		;unrelated code

org $C18EDF
	jml	ObjectSettingB

;routine setting
;$81/9141 B9 02 00    LDA $0002,y[$81:8CBD]   A:0004 X:0008 Y:8CBB P:envmxdIzc	;routine 1
;$81/9144 85 06       STA $06    [$00:0006]   A:91E4 X:0008 Y:8CBB P:eNvmxdIzc

org $C19141
	jml	ObjectSettingC

;thwomp hardcoded RR check removal
;819e2b lda $0124     [810124] A:0040 X:1000 Y:1800 S:1ff1 D:0000 DB:81 nvmxdizC V: 46 H:1098
;819e2e cmp #$0005             A:000d X:1000 Y:1800 S:1ff1 D:0000 DB:81 nvmxdizC V: 46 H:1270
;819e31 bne $9e37     [819e37] A:000d X:1000 Y:1800 S:1ff1 D:0000 DB:81 nvmxdizC V: 46 H:1288
;...
;819e37 rts                    A:000d X:1000 Y:1800 S:1ff1 D:0000 DB:81 nvmxdizC V: 46 H:1306

org $C19E2B
	jml	ThwompFix

;84dabc ldx $0d28     [840d28] A:0005 X:0008 Y:0000 S:1ff1 D:0000 DB:84 Nvmxdizc V:238 H:   2	;$0D28 index for jump table
;84dabf jsr ($daa9,x) [84daad] A:0005 X:0004 Y:0000 S:1ff1 D:0000 DB:84 nvmxdizc V:238 H:  36
;84dac2 plb                    A:0000 X:000e Y:0004 S:1fef D:0000 DB:84 nvmxdizc V:240 H: 106
;84dac3 rtl                    A:0000 X:000e Y:0004 S:1ff0 D:0000 DB:81 Nvmxdizc V:240 H: 132

org $C4DABC
	jml	ObjectLoadingA

;ghost valley code hacks

;84dca9 lda ($04),y	;load byte associated with this checkpoint
;84dcbd lda ($08),y   [84dd1d]	;read first word from $08 pointers (xpos)
;84dcc2 lda ($08),y

org $C4DCA9
	lda	[$04],y
org $C4DCBD
	lda	[$08],y
org $C4DCC2
	lda	[$08],y

;-----

reset bytes

org !target

;main object per course table

;my object defs
!PIPE		= 0
!PILLAR		= 1
!THWOMP 	= 2
!MOLE		= 3
!PLANT		= 4
!FISH		= 5
!RTHWOMP	= 6

print "table tileset: ",pc

ObjectsTileset:
	db !PIPE	;MC3
	db !PILLAR	;GV2
	db !MOLE	;DP2
	db !THWOMP	;BC2
	db !PIPE	;VL2
	db !RTHWOMP	;RR
	db !FISH	;KB2
	db !PIPE	;MC1
	db !PILLAR	;GV3
	db !THWOMP	;BC3
	db !PLANT	;CI2
	db !MOLE	;DP3
	db !PIPE	;VL1
	db !FISH	;KB1
	db !PIPE	;MC4
	db !PIPE	;MC2
	db !PILLAR	;GV1
	db !THWOMP	;BC1
	db !PLANT	;CI1
	db !PIPE	;DP1
;...
	db !PIPE,!PIPE,!PIPE,!PIPE

print "table interact: ",pc

ObjectsInteract:
	db !PIPE	;MC3
	db !PILLAR	;GV2
	db !MOLE	;DP2
	db !THWOMP	;BC2
	db !PIPE	;VL2
	db !RTHWOMP	;RR
	db !FISH	;KB2
	db !PIPE	;MC1
	db !PILLAR	;GV3
	db !THWOMP	;BC3
	db !PLANT	;CI2
	db !MOLE	;DP3
	db !PIPE	;VL1
	db !FISH	;KB1
	db !PIPE	;MC4
	db !PIPE	;MC2
	db !PILLAR	;GV1
	db !THWOMP	;BC1
	db !PLANT	;CI1
	db !PIPE	;DP1
;...
	db !PIPE,!PIPE,!PIPE,!PIPE

print "table routine: ",pc

ObjectsRoutine:
	db !PIPE	;MC3
	db !PILLAR	;GV2
	db !MOLE	;DP2
	db !THWOMP	;BC2
	db !PIPE	;VL2
	db !RTHWOMP	;RR
	db !FISH	;KB2
	db !PIPE	;MC1
	db !PILLAR	;GV3
	db !THWOMP	;BC3
	db !PLANT	;CI2
	db !MOLE	;DP3
	db !PIPE	;VL1
	db !FISH	;KB1
	db !PIPE	;MC4
	db !PIPE	;MC2
	db !PILLAR	;GV1
	db !THWOMP	;BC1
	db !PLANT	;CI1
	db !PIPE	;DP1
;...
	db !PIPE,!PIPE,!PIPE,!PIPE

print "table Z: ",pc

ObjectsZ:
	db !PIPE	;MC3
	db !PILLAR	;GV2
	db !MOLE	;DP2
	db !THWOMP	;BC2
	db !PIPE	;VL2
	db !RTHWOMP	;RR
	db !FISH	;KB2
	db !PIPE	;MC1
	db !PILLAR	;GV3
	db !THWOMP	;BC3
	db !PLANT	;CI2
	db !MOLE	;DP3
	db !PIPE	;VL1
	db !FISH	;KB1
	db !PIPE	;MC4
	db !PIPE	;MC2
	db !PILLAR	;GV1
	db !THWOMP	;BC1
	db !PLANT	;CI1
	db !PIPE	;DP1
;...
	db !PIPE,!PIPE,!PIPE,!PIPE

print "table loading: ",pc

ObjectsLoader:
	db !LOADER_NORM	;MC3
	db !LOADER_GV	;GV2
	db !LOADER_NORM	;DP2
	db !LOADER_NORM	;BC2
	db !LOADER_NORM	;VL2
	db !LOADER_NORM	;RR
	db !LOADER_FISH	;KB2
	db !LOADER_NORM	;MC1
	db !LOADER_GV	;GV3
	db !LOADER_NORM	;BC3
	db !LOADER_NORM	;CI2
	db !LOADER_NORM	;DP3
	db !LOADER_NORM	;VL1
	db !LOADER_FISH	;KB1
	db !LOADER_NORM	;MC4
	db !LOADER_NORM	;MC2
	db !LOADER_GV	;GV1
	db !LOADER_NORM	;BC1
	db !LOADER_NORM	;CI1
	db !LOADER_NORM	;DP1
;...
	db !LOADER_NONE,!LOADER_NONE,!LOADER_NONE,!LOADER_NONE

;---

;return with:
;OBJECT_INTERACT set
;A: bank byte pointer
;Y: low 16bits pointer

;16bit A/XY

print "A ptr: ",pc

ObjectSettingA:
	sep	#$30
	ldx.w	!course			;get object # for course
	lda.l	ObjectsInteract,x
	tax
	lda.l	.interactionTable,x
	sta.w	!object_interact	;16bit interaction set
	stz.w	!object_interact+1

	ldx.w	!course
	lda.l	ObjectsTileset,x
	tax
	lda.l	.graphicsTable,x
	pha				;x1.5 to get 24bit pointer
	lsr
	clc : adc $01,s : sta $01,s
	pla	
	rep 	#$30
	and	#$00FF
	tax
	lda.l	!object_gfx_ptrs,x	;graphics low 16bits
	tay
	lda.l	!object_gfx_ptrs+2,x	;graphics bank
	jml	$81E998

;interaction was normally just theme #, convert object # to their originating theme #
.interactionTable
	db !THEME_MC	;PIPE
	db !THEME_GV	;PILLAR
	db !THEME_BC	;THWOMP
	db !THEME_DP	;MOLE
	db !THEME_CI	;PLANT
	db !THEME_KB	;FISH
	db !THEME_BC	;RTHWOMP (RR shares the BC interaction for its thwomps)

;graphics was normally theme # too
.graphicsTable
	db !THEME_MC	;PIPE
	db !THEME_GV	;PILLAR
	db !THEME_BC	;THWOMP
	db !THEME_DP	;MOLE
	db !THEME_CI	;PLANT
	db !THEME_KB	;FISH
	db !THEME_RR	;RTHWOMP

;-----

;return with:
;$0FFC,0FFE set

;16bit A/XY

print "B ptr: ",pc

ObjectSettingB:
	sep	#$30
	ldx.w	!course		;get object # for course
	lda.l	ObjectsZ,x
	tax
	lda.l	.zTable,x	;get object Z index for table, normally just theme #
	tax
	rep	#$20		;16bit z values
	lda.l	!object_zbase_table,x
	sta.w	!object_zbase	;base stored
	lda.l	!object_zsize_table,x
	sta.w	!object_zsize
	rep	#$30
	jml	$818EEB

;index into Z tables in ROM
.zTable
	db !THEME_MC	;PIPE
	db !THEME_GV	;PILLAR
	db !THEME_BC	;THWOMP
	db !THEME_DP	;MOLE
	db !THEME_CI	;PLANT
	db !THEME_KB	;FISH
	db !THEME_BC	;RTHWOMP (RR shares the BC theme)

;-----

;return with:
;3 routines set

;16bit A/XY

print "C ptr: ",pc

ObjectSettingC:
	sep	#$30
	ldx.w	!course		;get object # for course
	lda.l	ObjectsRoutine,x
	asl			;x6
	pha
	asl
	clc : adc $01,s : sta $01,s
	plx
	rep	#$20		;16bit pointers
	lda.l	.routineTable+0,x
	sta	$06
	lda.l	.routineTable+2,x
	sta	$0F98
	sta	$08
	lda.l	.routineTable+4,x
	sta	$0F9A
	sta	$0A
	rep	#$10
	jml	$819156

.routineTable
	dw $91D0,$E4E7,$E4F7	;PIPE
	dw $91D0,$E6FE,$E708	;PILLAR
	dw $91D0,$E18B,$E19D	;THWOMP
	dw $91E4,$E36B,$E37B	;MOLE
	dw $91D0,$E06E,$E07A	;PLANT
	dw $91C0,$DF7E,$DF8E	;FISH
	dw $91D0,$E144,$E156	;RTHWOMP

;-----

;16bit: A/XY

print "D ptr: ",pc

ObjectLoadingA:
	sep	#$30
	ldx	!course			;course decides loader to use
	lda.l	ObjectsLoader,x
	asl
	tax
	jsr	(.routineTable,x)	;run loader
	rep	#$30
	jml	$84DAC2


.routineTable
	dw ObjectLoaderNorm	;LOADER_NORM
	dw ObjectLoaderFish	;LOADER_FISH
	dw ObjectLoaderGV	;LOADER_GV
	dw .return		;LOADER_NONE
.return
	rts	
;---

ObjectLoaderNorm:
	rep	#$30
	lda	!course		;course # decides object zone data
	asl	#2		;x10 bytes per course
	clc : adc !course
	asl
	clc : adc #.zoneData	;base location in ROM
	sta	$08		;low 16bits pointer
	lda.w	#!target>>16	
	sta	$0A		;bank
	ldy	$1EE4		;current screen
	ldx	$C8,y		;data for this screen
	bpl	+
.checkMirror
	lda 	$08		;ptr += 5 to access rear view zone data
	clc : adc.w #$0005
	sta 	$08
	tya
	eor 	#$0002
	tay
	ldx	$C8,y		;load alternate screen data
	bmi	.return		;return if this data is also invalid
+
	ldy	#$0000		;init index into object zone table
	sep	#$20
	lda	$C0,x		;load current checkpoint
	cmp	#$FF
	beq	.loadObjects
.checkTable
	dey
.loop
	iny
	cmp	[$08],y		;object zone reached yet?
	bcs	.loop
.loadObjects
	rep	#$20
	pea $006b	 ;push RTL + empty byte
	tsc	 ;need current SP for RTS parameter ((pointer to 6B) -1)
	phk : pea.w .return-1	;push 24bit return address
	pha	 ;push 16bit return address to SP
	jml	$84dc0b		;run original object position loader
.return
	pla	 ;pop RTL + empty byte
	rts

print "normal zone table: ",pc
.zoneData
	incbin normalzonedata.bin	;original zone data, 10 bytes per track

;---

ObjectLoaderFish:
	jsr	ObjectLoaderNorm	;reuse the regular loader
	bcc	.return
.loaded
	pea $006b	 ;push RTL + empty byte
	tsc	 ;need current SP for RTS parameter ((pointer to 6B) -1)
	phk : pea.w .retb-1	 ;push 24bit return address
	pha	 ;push 16bit return address to SP
	jml	$84DBBC	 ;run cheep followup routine
.retb
	pla
.return
	rts

;---

;$80 bytes to a checkpoint table
;$400 bytes to a position table

ObjectLoaderGV:
	rep	#$30
	ldy	$1EE4	;current screen
	ldx	$C8,y	;screen data pointer
	bmi	.return
.valid
	lda	$DC7C,y
	sta	$0C	;object size
	lda	!course
	xba		;x256
	lsr		;x128
	sta	$04	;checkpoint table offset
	asl	#3	;x1024
	clc : adc #.positionData
	sta	$08	;position table offset
	lda	$04
	clc : adc #.checkpointData
	sta	$04
	lda.w	#!target>>16	;bank
	sta	$06
	sta	$0A
.jump
	pea $006b	 ;push RTL + empty byte
	tsc	 ;need current SP for RTS parameter ((pointer to 6B) -1)
	phk : pea.w .retb-1	 ;push 24bit return address
	pha	 ;push 16bit return address to SP
	jml	$84dca3	 ;jump to original code
.retb
	pla
.return
	rts

print "GV checkpoint table: ",pc
.checkpointData
	incbin gvcheckpoint.bin		;original checkpoint data for GV
print "GV position data: ",pc
.positionData
	incbin gvposition.bin		;original pillar position data for GV

;-----

;16bit: A/XY

;return with:
;Z: 0 if RTHWOMP, 1 otherwise

print "E ptr: ",pc

ThwompFix:
	phx
	ldx	!course		;check interaction of this course
	sep	#$20
	lda.l	ObjectsInteract,x
	plx
	cmp.b	#!RTHWOMP	;is it an RR thwomp?
	rep	#$20
	jml	$819E31

;-----

print ""
print "total size: ",bytes," bytes"