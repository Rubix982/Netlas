import React from 'react';
import PropTypes from 'prop-types';
import Content from 'components/Login';

function SignInSide({ }) {
    return (
        <Content />
    );
}

SignInSide.propTypes = {
    location: PropTypes.object,
}

export default SignInSide;