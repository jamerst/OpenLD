import React, { Component } from 'react';
import { Route } from 'react-router';
import { Switch } from 'react-router-dom';
import { PageLayout, DrawingLayout } from './components/Layout';
import { Home } from './components/Home';
import { Drawing } from './components/Drawing';
import { FixtureLibrary } from './components/FixtureLibrary';
import { Token } from './components/Token';
import ApiAuthorizationRoutes from './components/api-authorization/ApiAuthorizationRoutes';
import { ApplicationPaths } from './components/api-authorization/ApiAuthorizationConstants';

import './FontAwesome';
import './custom.css'
// import './bootstrap-darkly.min.css'

const RouteLayout = ({ component: Component, layout: Layout, ...rest }) => (
  <Route {...rest} render={props => (
    <Layout>
      <Component {...props} />
    </Layout>
  )} />
);

export default class App extends Component {
  static displayName = App.name;

  render () {
    return (
      <Switch>
          <RouteLayout exact path='/' component={Home} layout={PageLayout} />
          <RouteLayout path='/library/:term?' component={FixtureLibrary} layout={PageLayout} />
          <RouteLayout path='/token' component={Token} layout={PageLayout} />
          <RouteLayout path={ApplicationPaths.ApiAuthorizationPrefix} component={ApiAuthorizationRoutes} layout={PageLayout} />
          <RouteLayout path='/drawing' component={Drawing} layout={DrawingLayout} />
      </Switch>
    );
  }
}
