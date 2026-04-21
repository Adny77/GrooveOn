// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'new_users_summary.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

NewUsersSummary _$NewUsersSummaryFromJson(Map<String, dynamic> json) =>
    NewUsersSummary(
      year: (json['year'] as num).toInt(),
      month: (json['month'] as num).toInt(),
      count: (json['count'] as num).toInt(),
    );

Map<String, dynamic> _$NewUsersSummaryToJson(NewUsersSummary instance) =>
    <String, dynamic>{
      'year': instance.year,
      'month': instance.month,
      'count': instance.count,
    };
