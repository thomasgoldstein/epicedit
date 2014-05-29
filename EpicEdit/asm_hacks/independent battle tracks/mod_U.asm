hirom

;16bit XY (preserve Y)
;16bit A

org $C18F14
	jml BattleHack

org $C80000

!COURSE		= $0124
!BATTLEBASE	= $14

!P1XPOS		= $1018
!P1YPOS		= $101C
!P1ZERO		= $1020
!P2XPOS		= $1118
!P2YPOS		= $111C
!P2ZERO		= $1120

;BC3: $14
;BC4: $15
;BC1: $16
;BC2: $17

BattleTable:
;BC3
	dw $0200,$0278	;P1
	dw $0200,$018A	;P2
;BC4
	dw $0200,$0278	;P1
	dw $0200,$0188	;P2
;BC1
	dw $0200,$0278	;P1
	dw $0200,$0188	;P2
;BC2
	dw $0200,$0278	;P1
	dw $0200,$0188	;P2

BattleHack:
	lda	!COURSE	;current course is a battle course?
	cmp.w	#!BATTLEBASE
	bcc	.normal
.battle
	sbc.w	#!BATTLEBASE
	asl 	#3	;8 bytes per course
	tax
	lda.l	BattleTable+0,x
	sta.w	!P1XPOS
	lda.l	BattleTable+2,x
	sta.w	!P1YPOS
	lda.l	BattleTable+4,x
	sta.w	!P2XPOS
	lda.l	BattleTable+6,x
	sta.w	!P2YPOS
	stz.w	!P1ZERO	;whatever these do, they are zero'd in this code
	stz.w	!P2ZERO
.return
	lda	!COURSE	;original course, without displacement
	asl	#1
	tax
	ldy	$8a79,x	;original code
	jml	$818f26	;just before the original iny #2
.normal
	asl 	a
	jml	$818f18