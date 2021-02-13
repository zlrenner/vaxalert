import React, { Component } from 'react';

export class Subscriptions extends Component {
  static displayName = Subscriptions.name;

  constructor(props) {
    super(props);
    this.state = { subscriptions: [], loading: true };
  }

  componentDidMount() {
    this.fetchData();
  }
  
  renderSubscription(subscription) {
    return (
      <tr key={subscription.key}>
        <td>{subscription.key}</td>
        <td>{subscription.title}</td>
      </tr>
    )
  }

  renderSubscriptionsTable(subscriptions) {
      return (
      <table className='table table-striped'>
        <thead>
          <tr>
            <th>EventKey</th>
            <th>Contact</th>
          </tr>
        </thead>
        <tbody>
          { subscriptions.map(this.renderSubscription) }
        </tbody>
      </table>
    );
  }

  render() {
    let subscriptions = this.state.loading
      ? <p><em>Loading...</em></p>
      : this.renderSubscriptionsTable(this.state.subscriptions);

    return (
      <div class="container">
        <h1>Add subscription</h1>
        <form>
          <div className="form-group row">
            <label htmlFor="email" className="col-sm-2 col-form-label">Email address</label>
            <div className="col-sm-10">
              <input type="email" className="form-control" id="email" placeholder="you@example.com (optional)" />
            </div>
          </div>
          <div className="form-group row">
            <label htmlFor="phone" className="col-sm-2 col-form-label">Phone</label>
            <div className="col-sm-10">
              <input type="phone" className="form-control" id="phone" placeholder="555-555-5555 (optional)" />
            </div>
          </div>
          <button type="button" className="btn btn-primary">Subscribe</button>
        </form>
        <br />
        <h1>Your subscriptions</h1>
        {subscriptions}
      </div>
    );
  }

  async fetchData() {
    const response = await fetch('api/subscriptions');
    const data = await response.json();
    this.setState({ subscriptions: data, loading: false });
  }
}
