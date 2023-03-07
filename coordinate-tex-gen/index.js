const fs = require('fs')
const path = require('path')
const { createCanvas } = require('canvas')
const StackBlur = require('stackblur-canvas')

const coordinateSets = [
    [4,0,2,3],
    [4,3,0],
    [3,5,1,2],
]

const rootDir = path.join(__dirname, '../../OuterWildsMod/OuterWildsUnityTemplate/Assets/ModAssets/Shared/Textures/Coordinates/')
fs.mkdirSync(rootDir, { recursive: true })

const SIZE = 512
const PADDING = 64.0
const VERTEX_RADIUS = 12.0
const EDGE_WIDTH = 24.0
const BLUR_RADIUS = 8.0
const HEX_RADIUS = (SIZE - PADDING * 2 - VERTEX_RADIUS) / 2
const PIP_SPACING = 64.0
const PIP_RADIUS = 16.0
const CENTER = SIZE / 2
const DEG_TO_RAD = Math.PI / 180

const canvas = createCanvas(SIZE, SIZE)
const ctx = canvas.getContext('2d')

function getVertexPos(coord) {
    const angle = (240 + 60 * coord) * DEG_TO_RAD
    const x = CENTER + Math.cos(angle) * HEX_RADIUS
    const y = CENTER + Math.sin(angle) * HEX_RADIUS
    return { x, y }
}

function drawVertex(coord) {
    const { x, y } = getVertexPos(coord)
    ctx.beginPath()
    ctx.arc(x, y, VERTEX_RADIUS, 0, Math.PI * 2)
    ctx.fill()
}

function drawEdge(coord0, coord1) {
    const p0 = getVertexPos(coord0)
    const p1 = getVertexPos(coord1)
    ctx.strokeStyle = 'white'
    ctx.lineWidth = EDGE_WIDTH
    ctx.beginPath()
    ctx.moveTo(p0.x, p0.y)
    ctx.lineTo(p1.x, p1.y)
    ctx.stroke()
}

function outputImage(coordSetIndex) {
    const filename = `COORD_${['X','Y','Z'][coordSetIndex]}.png`

    ctx.fillStyle = 'black'
    ctx.fillRect(0, 0, SIZE, SIZE)
    ctx.fillStyle = 'white'

    const currentSet = coordinateSets[coordSetIndex]
    
    for (let i = 0; i < currentSet.length; i++) {
        drawVertex(currentSet[i])
        if (i > 0) drawEdge(currentSet[i], currentSet[i - 1])
    }

    const imgData = ctx.getImageData(0, 0, SIZE, SIZE)
    StackBlur.imageDataRGB(imgData, 0, 0, SIZE, SIZE, BLUR_RADIUS)
    ctx.putImageData(imgData, 0, 0)

    const buffer = canvas.toBuffer('image/png')
    fs.writeFile(path.join(rootDir, filename), buffer, err => {
        if (err) console.error(err)
        else console.log('Saved', filename)
    })
}

for (let s = 0; s < coordinateSets.length; s++) {
    outputImage(s)
}
