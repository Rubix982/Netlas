import React from 'react'
import PropTypes from 'prop-types'
import Content from 'components/Forbidden'

function ForbiddenPage({ location }) {
    const pageTitle = location ? location.pathname.replace(/\//g, '') : ''
    return (
        <div className={{'margin': '0px', 'padding': '0px'}}>
            <Content />
        </div>
    )
}
ForbiddenPage.propTypes = {
    location: PropTypes.object,
}
export default ForbiddenPage
