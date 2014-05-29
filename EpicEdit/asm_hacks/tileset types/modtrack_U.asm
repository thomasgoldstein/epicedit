hirom

!target 	= $CA0000

;ram:
!course			= $0124
!behaviour_table	= $0B00

!LBorg_U	= $C1EB11
!LBorg_J	= $C1EB15
!LBorg		= !LBorg_U

;$7E0B00-$7E0BFF setting
;U:
;81eb11 ldy #$fdba
;81eb14 lda #$87
;81eb17 ldx #$0000
;81eb1a jsl $84e09e	;decompress data at $87FDBA to $7F0000+
;...
;81eb43 cpy #$0100	;finish copying last 64 tiles
;81eb46 bcc $eb3a
;J:
;81eb15 ldy #$fdba
;81eb18 lda #$0087
;81eb1b ldx #$0000
;81eb1e jsl $84df41
;...
;81eb47 cpy #$0100
;81eb4a bcc $eb25
;81eb4c rep #$20

org !LBorg
	jml LoadBehaviour
	
!LBret_U	= $C1EB48
!LBret		= !LBret_U

;---

!JBorg_U	= $C0B79E
!JBorg		= !JBorg_U

;jump bar checking
;U:
;80b79e lda $0126
;80b7a1 cmp #$000c	;*current theme is bowser castle?
;80b7a4 bne $b7af(skipBC)

org !JBorg
	jml JumpBarCheck

!JBret_U	= $C0B7A4
!JBret		= !JBret_U

;---

org !target

;return with:
;$7E0B00-$7E0BFF set to track behaviour

;8bit A
;16bit XY

print "LoadBehaviour: ", pc

LoadBehaviour:
	rep	#$20
	lda	!course			;course indexes behaviour table
	xba				;x256
	clc : adc.w #Behaviour		;add base address
	tax				;X: source
	ldy	#!behaviour_table	;Y: dest
	lda	#$0100-1		;A: size-1
	phb
	db	$54,$7E,Behaviour>>16	;256 byte copy
	plb
	jml	!LBret+2

;---

;return with:
;Z: 1 if we want jump bars to slow you down

;16bit AXY

print "JumpBarCheck: ", pc

JumpBarCheck:
	phx
	ldx	!course
	lda.l	JumpBar,x	;read jump bar flag for this course
	plx
	and	#$00FF
	jml	!JBret

;---

;tables:

print "behaviour tables: ", pc

Behaviour:
incbin mcbt.bin ;$00: MC3
incbin gvbt.bin ;$01: GV2
incbin dpbt.bin ;$02: DP2
incbin bcbt.bin ;$03: BC2
incbin vlbt.bin ;$04: VL2
incbin rrbt.bin ;$05: RR
incbin kbbt.bin ;$06: KB2
incbin mcbt.bin ;$07: MC1
incbin gvbt.bin ;$08: GV3
incbin bcbt.bin ;$09: BC3
incbin cibt.bin ;$0A: CI2
incbin dpbt.bin ;$0B: DP3
incbin vlbt.bin ;$0C: VL1
incbin kbbt.bin ;$0D: KB1
incbin mcbt.bin ;$0E: MC4
incbin mcbt.bin ;$0F: MC2
incbin gvbt.bin ;$10: GV1
incbin bcbt.bin ;$11: BC1
incbin cibt.bin ;$12: CI1
incbin dpbt.bin ;$13: DP1
incbin vlbt.bin ;$14: BC3
incbin mcbt.bin ;$15: BC4
incbin dpbt.bin ;$16: BC1
incbin kbbt.bin ;$17: BC2

print "jump bar table: ", pc

JumpBar:
	db 1,1,1,0,1	;MC3,GV2,DP2,BC2,VL2
	db 1,1,1,1,0	;RR,KB2,MC1,GV3,BC3
	db 1,1,1,1,1	;CI2,DP3,VL1,KB1,MC4
	db 1,1,0,1,1	;MC2,GV1,BC1,CI1,DP1
	db 1,1,1,1	;Battle
	