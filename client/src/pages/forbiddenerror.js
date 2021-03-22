import React from 'react'
import PropTypes from 'prop-types'
import Layout from 'components/Layout'
import Content from 'components/Content'

function ForbiddenErrorPage({ location }) {
	const pageTitle = location ? location.pathname.replace(/\//g, '') : ''
	return (
		// <Layout location={location} title={pageTitle}>
			<p>Error 500!</p>
		// </Layout>
	)
}
ForbiddenErrorPage.propTypes = {
	location: PropTypes.object,
}
export default ForbiddenErrorPage
