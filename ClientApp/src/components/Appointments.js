import React, { Component } from 'react';
import moment from 'moment';

export class Appointments extends Component {
  static displayName = Appointments.name;

  constructor(props) {
    super(props);
    this.state = { appointments: [], loading: true };
  }

  componentDidMount() {
    this.fetchData();
  }
  
  renderAppointment(appointment) {
    return (
      <tr key={appointment.key}>
        <td style={{width: "25%"}}>{appointment.source}</td>
        <td>{appointment.location}</td>
        {/*<td>{appointment.date ? moment(appointment.date).format('MMM Do (dddd)') : ""}</td>*/}
        {/*<td>{appointment.time ? moment(appointment.time).format('h:mm A') : ""}</td>*/}
        {/*  */}
        <td>{appointment.title}</td>
        <td style={{ color: "darkgray" }}>{appointment.notes}</td>
        <td>{appointment.scanTime ? moment(appointment.scanTime).fromNow() : ""}</td>
      </tr>
    )
  }
  
  availableAppointments(appointment) {
      return appointment.available;
  }

  renderAppointmentsTable(appointments) {
      let availableAppointments = appointments.filter(this.availableAppointments);
      let deadAppointments = appointments.filter(x => !this.availableAppointments(x));
      
      return (
      <table className='table table-striped'>
        <thead>
          <tr>
            <th style={{width: "25%"}}>Type</th>
            <th>Location</th>
            <th>Title</th>
            <th>Notes</th>
            <th>Last Checked</th>
          </tr>
        </thead>
        <tbody>
          { availableAppointments.map(this.renderAppointment) }
        </tbody>
          {
              deadAppointments.length > 0 ?
              <tbody style={{"color": "#CCCCCC"}}>
              <td>No longer available...</td>
              { deadAppointments.map(this.renderAppointment) }
              </tbody> : null
          }
      </table>
    );
  }

  render() {
    let contents = this.state.loading
      ? <p><em>Loading...</em></p>
      : this.renderAppointmentsTable(this.state.appointments);

    return (
      <div>
        <h1>Available Appointments</h1>
        {contents}
      </div>
    );
  }

  async fetchData() {
    const response = await fetch('api/appointments?includeExpired=true');
    const data = await response.json();
    this.setState({ appointments: data, loading: false });
  }
}
