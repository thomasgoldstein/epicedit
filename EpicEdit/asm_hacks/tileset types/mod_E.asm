hirom

!target 	= $CA0000

;ram:
!course			= $0126
!behaviour_table	= $0B00

!LBorg_U	= $C1EB11
!LBorg_J	= $C1EB15
!LBorg_E	= $C1EB00
!LBorg		= !LBorg_E

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
;E:
;81eb00 ldy #$fdba
;81eb03 lda #$0087
;81eb06 ldx #$0000
;81eb09 jsl $84df41
;...
;81eb32 cpy #$0100
;81eb35 bcc $eb29

org !LBorg
	jml LoadBehaviour
	
!LBret_U	= $C1EB48
!LBret_J	= $C1EB4C
!LBret_E	= $C1EB37
!LBret		= !LBret_E

;---

!JBorg_U	= $C0B79E
!JBorg_J	= $C0B795
!JBorg_E	= $C0B7A3
!JBorg		= !JBorg_E

;jump bar checking
;U:
;80b79e lda $0126
;80b7a1 cmp #$000c	;*current theme is bowser castle?
;80b7a4 bne $b7af(skipBC)
;J:
;80b795 lda $0126
;80b798 cmp #$000c
;80b79b bne $b7af(skipBC)
;E:
;80b7a3 lda $0126
;80b7a6 cmp #$000c
;80b7a9 bne $b7bd(skipBC)


org !JBorg
	jml JumpBarCheck

!JBret_U	= $C0B7A4
!JBret_J	= $C0B79B
!JBret_E	= $C0B7A9
!JBret		= !JBret_E

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
	lsr
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
	lda !course
	lsr
	tax
	lda.l	JumpBar,x	;read jump bar flag for this course
	plx
	and	#$00FF
	jml	!JBret

;---

;tables:

print "behaviour tables: ", pc

Behaviour:
incbin gvbt.bin ;$00: GV
incbin mcbt.bin ;$01: MC
incbin dpbt.bin ;$02: DP
incbin cibt.bin ;$03: CI
incbin vlbt.bin ;$04: VL
incbin kbbt.bin ;$05: KB
incbin bcbt.bin ;$06: BC
incbin rrbt.bin ;$07: RR

print "jump bar table: ", pc

JumpBar:
	db 1,1,1,1,1,1,0,1	;GV,MC,DP,CI,VL,KB,BC,RR
	