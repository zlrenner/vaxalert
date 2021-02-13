import React, { Component } from 'react';
import { Route } from 'react-router';
import { Layout } from './components/Layout';
import { Home } from './components/Home';
import { Appointments } from './components/Appointments';
import { Subscriptions } from "./components/Subscriptions";
import { Notifications } from "./components/Notifications";

import './custom.css'

export default class App extends Component {
  static displayName = App.name;

  render () {
    return (
      <Layout>
        <Route exact path='/' component={Home} />
        <Route path='/appointments' component={Appointments} />
        <Route path='/subscriptions' component={Subscriptions} />
        <Route path='/notifications' component={Notifications} />
      </Layout>
    );
  }
}
