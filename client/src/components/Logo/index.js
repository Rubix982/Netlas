import React, { memo } from 'react'

function Logo() {
	return (
		<img src="images/synet.svg" style={{'width': '60px', 'height': '60px'}} />
	)
}

export default memo(Logo)
